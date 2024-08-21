using System;
using System.Collections.Generic;
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
            
            attributes.AddRange(property.GetCustomAttributes());

            var parameter = GetCorrespondingParameter(property);

            if (parameter is { } p)
            {
                attributes.AddRange(p.GetCustomAttributes());
            }

            return attributes;
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