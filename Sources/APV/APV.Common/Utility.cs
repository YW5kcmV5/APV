using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace APV.Common
{
    public static class Utility
    {
        #region Hash

        public static byte[] Hash256(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var hashEvidence = new Hash(assembly);
            byte[] hashCode = hashEvidence.SHA256;
            return hashCode;
        }

        public static byte[] Hash1(string data, Encoding encoding = null)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            encoding = encoding ?? Encoding.UTF8;
            byte[] rawData = encoding.GetBytes(data);
            return Hash1(rawData);
        }

        public static byte[] Hash256(string data, Encoding encoding = null)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            encoding = encoding ?? Encoding.UTF8;
            byte[] rawData = encoding.GetBytes(data);
            return Hash256(rawData);
        }

        public static byte[] Hash1(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (SHA1 sha = new SHA1Managed())
            {
                return sha.ComputeHash(data);
            }
        }

        public static byte[] Hash256(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (SHA256 sha = new SHA256Managed())
            {
                return sha.ComputeHash(data);
            }
        }

        #endregion

        public static long GetLongHashCode(this string data, Encoding encoding = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            encoding = encoding ?? Encoding.UTF8;

            byte[] binaryData = encoding.GetBytes(data);
            byte[] hashData;
            using (SHA256 sha = new SHA256Managed())
            {
                hashData = sha.ComputeHash(binaryData);
            }
            Int64 hashCode0 = BitConverter.ToInt64(hashData, 0);
            Int64 hashCode1 = BitConverter.ToInt64(hashData, 8);
            Int64 hashCode2 = BitConverter.ToInt64(hashData, 16);
            Int64 hashCode3 = BitConverter.ToInt64(hashData, 24);
            Int64 hashCode = hashCode0 ^ hashCode1 ^ hashCode2 ^ hashCode3;
            return hashCode;
        }

        #region Checksum

        public static string GetChecksum(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            byte[] hash = Hash256(assembly);
            return ToHexString(hash);
        }

        public static string GetChecksum(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            byte[] hash = Hash256(value);
            return ToHexString(hash);
        }

        public static string GetChecksum(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            byte[] hash = Hash256(value);
            return ToHexString(hash);
        }

        #endregion


        public static bool BasedOn(this Type objectType, Type baseType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");
            if (baseType == null)
                throw new ArgumentNullException("baseType");

            if (baseType.IsInterface)
            {
                return objectType.GetInterfaces().Any(@interface => @interface == baseType);
            }

            do
            {
                objectType = objectType.BaseType;
                if (objectType == baseType)
                {
                    return true;
                }
            } while (objectType != null);

            return false;
        }

        public static bool BasedOn<T>(this Type objectType)
        {
            return BasedOn(objectType, typeof(T));
        }

        public static bool HasDefaultConstructor(this Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            ConstructorInfo constructor = objectType.GetConstructor(Type.EmptyTypes);
            return (constructor != null);
        }

        public static string ExtractName<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Unrecognized expression", "expression");

            return memberExpression.Member.Name;
        }

        public static string ExtractName(this Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            //if (expression is UnaryExpression)
            //{
            //    var unaryExpression = (UnaryExpression)expression;
            //    return ExtractName(unaryExpression);
            //}

            throw new ArgumentException("Unrecognized expression", "expression");
        }

        public static string ToTraceString(this Exception ex, bool addNewLine = true)
        {
            var cr = (addNewLine) ? Environment.NewLine : string.Empty;
            if (ex == null)
            {
                return string.Empty;
            }
            var message = string.Format("Message: '{0}' Type: '{1}'{2}", ex.Message, ex.GetType(), cr);
            message += string.Format(" StackTrace: '{0}'{1}", ex.StackTrace, cr);
            if (ex.InnerException != null)
            {
                message += string.Format(" InnerExceptionMessage:{0}{1}", cr, ex.InnerException.ToTraceString());
            }
            message = message.Trim() + cr;
            return message;
        }

        public static string ToHexString(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            int length = data.Length;
            var hex = new StringBuilder(length * 2);
            for (int i = 0; i < length; i++)
            {
                hex.AppendFormat("{0:X2}", data[i]);
            }
            return hex.ToString();
        }

        public static byte[] Copy(this byte[] from)
        {
            if (from == null)
            {
                return null;
            }
            var newAr = new byte[from.Length];
            from.CopyTo(newAr, 0);
            return newAr;
        }

        public static string[] Copy(this string[] from)
        {
            if (from == null)
            {
                return null;
            }
            var newAr = new string[from.Length];
            from.CopyTo(newAr, 0);
            return newAr;
        }

        public static XmlDocument Copy(this XmlDocument from)
        {
            if (from == null)
            {
                return null;
            }
            var doc = new XmlDocument();
            doc.LoadXml(from.OuterXml);
            return doc;
        }

        public static void Invoke(Action actionToExecute, int attempts = 1)
        {
            Func<bool> func = () =>
                {
                    actionToExecute();
                    return true;
                };
            Invoke(func, attempts);
        }

        public static T Invoke<T>(Func<T> actionToExecute, int attempts = 1)
        {
            if (attempts <= 0)
            {
                attempts = 1;
            }
            if (attempts == 1)
            {
                return actionToExecute();
            }
            while (true)
            {
                attempts--;
                try
                {
                    return actionToExecute();
                }
                catch (Exception)
                {
                    if (attempts == 0)
                    {
                        throw;
                    }
                }
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
                email = email.Trim().ToLowerInvariant();
                var addr = new System.Net.Mail.MailAddress(email);
                return (addr.Address == email);
            }
            catch
            {
                return false;
            }
        }
    }
}