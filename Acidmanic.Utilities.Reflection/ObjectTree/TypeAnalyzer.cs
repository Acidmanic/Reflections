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