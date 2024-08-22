using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Acidmanic.Utilities.Reflection.Extensions
{
    public static class PropertyInfoAttributeExtensions
    {



        public static bool IsTreatAsALeaf(this PropertyInfo property)
        {
            var attributes = GetCustomAttributeParametersIncluded<TreatAsLeafAttribute>(property);

            return attributes.Count > 0;
        }

        
        public static List<TAttribute> GetCustomAttributeParametersIncluded<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
        {
            return GetCustomAttributesParametersIncluded(property)
                .OfType<TAttribute>().Where(a => a != null).Select(a => a!).ToList();
        }
        
        
        
        
        public static List<Attribute> GetCustomAttributesParametersIncluded(this PropertyInfo property)
        {
            var attributes = new List<Attribute>();

            var ancestors = GetAncestors(property);

            foreach (var ancestor in ancestors)
            {
                attributes.AddRange(ancestor.GetCustomAttributes());
            }

            var parameter = GetCorrespondingParameter(property);

            if (parameter is { } p)
            {
                attributes.AddRange(p.GetCustomAttributes());
            }

            return attributes;
        }
        
        
        private static List<PropertyInfo> GetAncestors(PropertyInfo propertyInfo)
        {
            var ancestorsTypes = new List<Type>();
            
            ancestorsTypes.AddRange(propertyInfo.DeclaringType?.GetInterfaces()?? new Type[]{});
            
            ancestorsTypes.AddRange(propertyInfo.DeclaringType?.BaseHierarchyBranch() ?? new List<Type>());

            var ancestors = new List<PropertyInfo>();

            foreach (var ancestorsType in ancestorsTypes)
            {
                ancestors.AddRange(ancestorsType.GetProperties()
                    .Where(p => p.Name==propertyInfo.Name));
            }

            return ancestors;
        }


        public static List<Type> BaseHierarchyBranch(this Type type)
        {
            var hierarchy = new List<Type>();

            Type? parent = type;
            
            while (parent is {} p)
            {
                hierarchy.Add(p);

                parent = p.BaseType;
            }

            return hierarchy;
        }

        public static ParameterInfo? GetCorrespondingParameter(this PropertyInfo property)
        {
            var type = property.DeclaringType;

                var parametersInfo = new List<ParameterInfo>();
                
            if (type is { } t)
            {
                
                var constructors = t.GetConstructors();

                foreach (var constructor in constructors)
                {
                    parametersInfo.AddRange(constructor.GetParameters());    
                }
                
            }
            
            foreach (var p in parametersInfo)
            {
                if (p.Name == property.Name && p.ParameterType == property.PropertyType)
                {
                    return p;
                }
            }
            
            return null;
        }
    }
}