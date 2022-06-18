using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class TypeAnalyzer
    {
        public IDataOwnerNameProvider DataOwnerNameProvider { get; set; } = new PluralDataOwnerNameProvider();

        public AccessNode ToAccessNode(Type type, bool fullTree = false)
        {
            return ObjectStructure.CreateStructure(type, fullTree);
        }

        


        public string GetMappedName(PropertyInfo property)
        {
            string name = property.Name;
            //TODO: Put naming conventions here   

            List<MemberNameAttribute> attributes = new List<MemberNameAttribute>();

            attributes.AddRange(property.GetCustomAttributes<MemberNameAttribute>());

            if (attributes.Count > 0)
            {
                name = attributes.Last().Name;
            }

            return name;
        }


        public AccessNode ToAccessNode<T>(bool fullTree = false)
        {
            return ToAccessNode(typeof(T), fullTree);
        }


        public Dictionary<string, int> CountFieldNames(List<AccessNode> nodes)
        {
            Dictionary<string, int> fieldCount = new Dictionary<string, int>();

            foreach (var node in nodes)
            {
                var field = node.Name;

                if (fieldCount.ContainsKey(field))
                {
                    fieldCount[field] += 1;
                }
                else
                {
                    fieldCount.Add(field, 1);
                }
            }

            return fieldCount;
        }


        public string GetFieldName<TModel>(MemberExpression expression)
        {
            if (expression.Member.MemberType == MemberTypes.Property)
            {
                var counts = CountLeafMemberNames<TModel>();

                var name = expression.Member.Name;

                if (counts.ContainsKey(name) && counts[name] > 1)
                {
                    name = DataOwnerNameProvider.GetNameForOwnerType(expression.Member.DeclaringType) + "." + name;
                }

                return name;
            }

            return null;
        }

        private Dictionary<string, int> CountLeafMemberNames<TModel>()
        {
            var result = new Dictionary<string, int>();

            CountLeafMemberNames(typeof(TModel), result);

            return result;
        }

        private void CountLeafMemberNames(Type type, Dictionary<string, int> result)
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
                    CountLeafMemberNames(pType, result);
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

        public TOut CreateObject<TOut>(bool fullTree)
        {
            var type = typeof(TOut);

            return (TOut) CreateObject(type, fullTree);
        }

        public object CreateObject(Type type, bool fullTree)
        {
            if (fullTree)
            {
                return CreateObject(type);
            }

            return BlindInstantiate(type);
        }

        public object BlindInstantiate(Type type)
        {
            return type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
        }

        public object CreateObject(Type type)
        {
            var obj = BlindInstantiate(type);

            if (TypeCheck.IsCollection(type))
            {
                return obj;
            }

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                if (TypeCheck.IsReferenceType(pType))
                {
                    object value;

                    if (TypeCheck.IsCollection(pType))
                    {
                        value = BlindInstantiate(pType);
                    }
                    else if (pType.IsArray)
                    {
                        value = Array.CreateInstance(pType.GetElementType() ?? typeof(object), 0);
                    }
                    else
                    {
                        value = CreateObject(pType);
                    }

                    property.SetValue(obj, value);
                }
            }

            return obj;
        }
    }
}