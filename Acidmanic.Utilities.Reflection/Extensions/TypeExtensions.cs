using System;
using System.Collections.Generic;
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


        public static List<Type> GetBaseTypesHierarchy(this Type type)
        {
            var hierarchy = new List<Type>();

            var currentType = type.BaseType;

            while (currentType != null)
            {
                hierarchy.Add(currentType);

                currentType = currentType.BaseType;
            }

            return hierarchy;
        }
    }
}