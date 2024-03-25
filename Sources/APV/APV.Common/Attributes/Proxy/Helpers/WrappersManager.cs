using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using APV.Common.Attributes.Proxy.Attributes;
using APV.Common.Extensions;

namespace APV.Common.Attributes.Proxy.Helpers
{
    internal static class WrappersManager
    {
        private static readonly SortedList<int, Type> Wrappers = new SortedList<int, Type>();

        private static Type GenerateWrapper(Type serverType)
        {
            PropertyInfo[] properties = serverType.GetAllProperties();
            properties = properties.Where(item => item.GetGetMethod(true).IsVirtual && item.GetCustomAttribute<BaseWraperAttribute>() != null).ToArray();
            if (properties.Length == 0)
            {
                return serverType;
            }

            Assembly current = serverType.Assembly;
            string newAssemblyName = current.GetName().Name + "Wrapped";
            string newClassName = serverType.Name + "W";

            var namespaces = new List<string>();

            string newClass =
                "namespace " + serverType.Namespace + "\r\n" +
                "{" + "\r\n" +
                "	public class " + newClassName + " : " + serverType.Name + "\r\n" +
                "	{" + "\r\n";

            //Copy constructors
            ConstructorInfo[] constructors = serverType.GetConstructors();
            for (int i = 0; i < constructors.Length; i++)
            {
                ConstructorInfo constructor = constructors[i];
                ParameterInfo[] @params = constructor.GetParameters();
                string p = "protected";
                if (constructor.IsPrivate)
                {
                    p = "private";
                }
                else if (constructor.IsPublic)
                {
                    p = "public";
                }
                string argsWithType = string.Empty;
                string args = string.Empty;
                for (int j = 0; j < @params.Length; j++)
                {
                    ParameterInfo parameter = @params[j];
                    string parameterType = parameter.ParameterType.Name;
                    string parameterNamespace = parameter.ParameterType.Namespace;
                    if ((!namespaces.Contains(parameterNamespace)) && (parameterNamespace != serverType.Namespace))
                    {
                        namespaces.Add(parameterNamespace);
                    }
                    argsWithType += parameterType + " " + parameter.Name;
                    args += parameter.Name;
                    if (j != @params.Length - 1)
                    {
                        argsWithType += ", ";
                        args += ", ";
                    }
                }
                newClass += "		" + p + " " + newClassName + "(" + argsWithType + ")" + "\r\n";
                newClass += "			: base(" + args + ")" + "\r\n";
                newClass += "		{" + "\r\n";
                newClass += "		}" + "\r\n";
                newClass += "" + "\r\n";
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                string name = property.Name;
                //int propertyNameHash = name.GetHashCode();
                string type = property.PropertyType.Name;
                string typeNamespace = property.PropertyType.Namespace;
                if ((!namespaces.Contains(typeNamespace)) && (typeNamespace != serverType.Namespace))
                {
                    namespaces.Add(typeNamespace);
                }

                var attribute = property.GetCustomAttribute<BaseWraperAttribute>(true);
                string getValue = (attribute.GetPropertyGetCode(property) ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(getValue))
                {
                    getValue = $"return base.{name};\r\n";
                }
                if (!getValue.EndsWith(";"))
                {
                    getValue += ";";
                }

                newClass +=
                    "		public override " + type + " " + name + "\r\n" +
                    "		{" + "\r\n" +
                    "			get" + "\r\n" +
                    "			{" + "\r\n" +
                    //"				return (" + type + ") Cache.GetValue(" + propertyNameHash + ", " + attribute.DisposeWhenClear.ToString().ToLower() + ", " + attribute.LiveTimeInSeconds + ", " + attribute.Clone.ToString().ToLower() + ", () => base." + name + ");" + "\r\n" +
                    "               " + getValue + "\r\n" +
                    "			}" + "\r\n" +
                    "		}" + "\r\n" +
                    "" + "\r\n";
            }
            newClass +=
                "	}" + "\r\n" +
                "}" + "\r\n";

            string namespacesStr = string.Empty;
            for (int i = 0; i < namespaces.Count; i++)
            {
                namespacesStr += "using " + namespaces[i] + ";" + "\r\n";
            }
            newClass = namespacesStr + newClass;

            //var values = new Dictionary<string, string>();
            var values = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
            var provider = new CSharpCodeProvider(values);

            var parameters = new CompilerParameters
                {
                    GenerateExecutable = false,
                    OutputAssembly = newAssemblyName + ".dll"
                };
            parameters.ReferencedAssemblies.Add(current.Location);
            List<AssemblyName> referencedAssemblies = current.GetReferencedAssemblies().ToList();
            Assembly[] domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in domainAssemblies)
            {
                if (referencedAssemblies.All(name => name.Name != assembly.GetName().Name))
                {
                    referencedAssemblies.Add(assembly.GetName());
                }
            }
            foreach (AssemblyName name in referencedAssemblies)
            {
                Assembly referencedAssembly = Assembly.Load(name);
                parameters.ReferencedAssemblies.Add(referencedAssembly.Location);
            }

            //var icc = provider.CreateCompiler();
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, newClass);

            if (results.Errors.HasErrors)
                throw new InvalidOperationException($"Wrapper can not be compiled.\r\nErrorNumber={results.Errors[0].ErrorNumber}\r\n{results.Errors[0].ErrorText}");

            Assembly compiledAssembly = results.CompiledAssembly;
            string wtypeName = serverType.Namespace + "." + newClassName;
            Type wtype = compiledAssembly.GetType(wtypeName);

            if (wtype == null)
                throw new InvalidOperationException($"Wrapped type \"{wtypeName}\" can not be found in assembly \"{compiledAssembly.FullName}\".");

            return wtype;
        }

        public static Type GetWrapper(Type type)
        {
            lock (Wrappers)
            {
                int key = type.FullName.GetHashCode();
                int index = Wrappers.IndexOfKey(key);
                if (index != -1)
                {
                    return Wrappers.Values[index];
                }
                Type wrapper = GenerateWrapper(type);
                Wrappers.Add(key, wrapper);
                return wrapper;
            }
        }
    }
}