using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Acidmanic.Utilities.Reflection
{
    public class ObjectInstantiator
    {
        public TOut CreateObject<TOut>(bool fullTree)
        {
            var type = typeof(TOut);

            return (TOut)CreateObject(type, fullTree);
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
            if (TypeCheck.IsEffectivelyPrimitive(type))
            {
                return InstantiatePrimitive(type);
            }

            Func<object> constructor = null;
            
            
            if (type.IsAbstract || type.IsInterface)
            {
                constructor = FindConcreteConstructor(type);
            }
            else
            {
                constructor = FindPrimitiveConstructor(type);
            }

            object createdObject = null;

            try
            {
                createdObject = constructor.Invoke();
            }
            catch (Exception _)
            {
            }

            // Check for cases that a wrong concrete class has been found
            if (createdObject != null && !type.IsInstanceOfType(createdObject))
            {
                createdObject = null;
            }

            return createdObject;
        }


        private Func<object> FindPrimitiveConstructor(Type type)
        {
            var constructors = type.GetConstructors()
                .OrderBy(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                if (IsFullyPrimitive(constructor))
                {
                    var parameters = constructor.GetParameters();

                    var values = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        values[i] = InstantiatePrimitive(parameters[i].ParameterType);
                    }

                    return () => constructor.Invoke(values);
                }
            }

            return null;
        }

        private object InstantiatePrimitive(Type type)
        {
            if (type == typeof(string))
            {
                return default(string);
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return default;
        }

        private bool IsFullyPrimitive(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            foreach (var parameter in parameters)
            {
                if (!TypeCheck.IsEffectivelyPrimitive(parameter.ParameterType))
                {
                    return false;
                }
            }

            return true;
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
                    else if (pType == type)
                    {
                        // To avoid infinite recursions
                        value = null;
                    }
                    else if (property.GetCustomAttribute<TreatAsLeafAttribute>() != null)
                    {
                        // Leafs should not go deeper than one level
                        value = BlindInstantiate(pType);
                    }
                    else
                    {
                        value = CreateObject(pType);
                    }

                    if (property.CanWrite)
                    {
                        property.SetValue(obj, value);    
                    }
                }
            }

            return obj;
        }
    }
}