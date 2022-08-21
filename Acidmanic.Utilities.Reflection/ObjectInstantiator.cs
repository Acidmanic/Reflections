using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;

namespace Acidmanic.Utilities.Reflection
{
    public class ObjectInstantiator
    {
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
            var constructor = FindConcreteConstructor(type);

            if (type == typeof(string))
            {
                return default(string);
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return constructor.Invoke();
        }

        private Func<object> FindConcreteConstructor(Type type)
        {
            var constructor = HasConstructableImplementation(type);

            if (constructor != null)
            {
                return constructor;
            }

            var descendants = FindDescendants(type);

            foreach (var descendant in descendants)
            {
                var constructable = FindConcreteConstructor(descendant);

                if (constructable != null)
                {
                    return constructable;
                }
            }

            return null;
        }

        private Func<object> HasConstructableImplementation(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (elementType == null)
                {
                    elementType = typeof(object);
                }

                return () => Array.CreateInstance(elementType, 0);
            }

            if (TypeCheck.IsCollection(type))
            {
                var elementType = TypeCheck.GetElementType(type);

                var listType = typeof(List<>).MakeGenericType(elementType);

                var constructor = listType.GetConstructor(new Type[] { });

                if (constructor != null)
                {
                    return () => constructor.Invoke(new object[] { });
                }
            }

            if (!type.IsInterface && !type.IsAbstract)
            {
                var constructor = type.GetConstructor(new Type[] { });

                if (constructor != null)
                {
                    return () => constructor.Invoke(new object[] { });
                }
            }

            return null;
        }

        private List<Type> FindDescendants(Type type)
        {
            var availableTypes = new List<Type>();

            availableTypes.AddRange(Assembly.GetExecutingAssembly().GetAvailableTypes());
            availableTypes.AddRange(Assembly.GetCallingAssembly().GetAvailableTypes());
            availableTypes.AddRange(Assembly.GetEntryAssembly().GetAvailableTypes());

            return availableTypes.Where(driven => TypeCheck.InheritsFrom(type, driven) && driven != type)
                .ToList();
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