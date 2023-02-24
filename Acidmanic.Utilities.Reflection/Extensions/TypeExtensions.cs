using System;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Acidmanic.Utilities.Reflection.Extensions
{
    public static class TypeExtensions
    {




        public static Type GetAlteredOrOriginal(this Type type)
        {

            var alterAttribute = type.GetCustomAttribute<AlteredTypeAttribute>();

            if (alterAttribute != null)
            {
                return alterAttribute.AlternativeType;
            }

            return type;
        }

    }
}