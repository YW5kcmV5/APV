using System;

namespace APV.EntityFramework.BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AnonymousAccessAttribute : MethodAccessAttribute
    {
    }
}