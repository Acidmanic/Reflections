using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Acidmanic.Utilities.Reflection
{
    /// <summary>
    /// This class provides functionalities to deal with a class as a data and it's fields as the members of this data
    /// and their standard or costume (provided using attributes) names 
    /// </summary>
    public class MemberOwnerUtilities
    {
        public MemberOwnerUtilities(IDataOwnerNameProvider dataOwnerNameProvider)
        {
            DataOwnerNameProvider = dataOwnerNameProvider;
        }


        public IDataOwnerNameProvider DataOwnerNameProvider { get; }
        
        public string GetFieldName<TModel>(MemberExpression expression, bool fullTree = false)
        {
            if (expression.Member.MemberType == MemberTypes.Property)
            {
                var counts = CountLeafMemberNames<TModel>(fullTree);

                var name = expression.Member.Name;

                
                
                if (counts.ContainsKey(name) && counts[name] > 1  )
                {
                    name = DataOwnerNameProvider.GetNameForOwnerType(expression.Member.DeclaringType) + "." + name;
                }

                return name;
            }

            return null;
        }
        
        private Dictionary<string, int> CountLeafMemberNames<TModel>(bool fullTree )
        {
            var result = new Dictionary<string, int>();

            CountLeafMemberNames(typeof(TModel), result,fullTree);

            return result;
        }

        private void CountLeafMemberNames(Type type, Dictionary<string, int> result,bool fullTree)
        {
            //TODO: cache here
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;
                var refType = TypeCheck.IsReferenceType(property.PropertyType);
                var name = refType ? DataOwnerNameProvider.GetNameForOwnerType(pType) : GetMappedName(property);

                if (refType)
                {
                    if (fullTree)
                    {
                        CountLeafMemberNames(pType, result,true);   
                    }
                }
                else
                {
                    if (result.ContainsKey(name))
                    {
                        result[name]++;
                    }
                    else
                    {
                        result.Add(name, 1);
                    }
                }
            }
        }
        
        public string GetMappedName(PropertyInfo property)
        {
            string name = property.Name;
            //TODO: Put naming conventions here   

            var attributes = new List<MemberNameAttribute>();

            attributes.AddRange(property.GetCustomAttributes<MemberNameAttribute>());

            if (attributes.Count > 0)
            {
                name = attributes.Last().Name;
            }

            return name;
        }

    }
}