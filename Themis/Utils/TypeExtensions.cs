using System;

namespace Themis.Utils
{
    public static class TypeExtensions
    {
        public static bool IsOpenGeneric(this Type type)
        {
            return type.IsGenericType && type.IsGenericTypeDefinition;
        }
    }
}