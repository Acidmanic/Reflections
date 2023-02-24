using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Acidmanic.Utilities.Reflection
{
    public static class TypeCheck
    {
        private static readonly Dictionary<Type, bool> IsModelCache = new Dictionary<Type, bool>();


        public static bool IsCollection(Type type)
        {
            return Implements<ICollection>(type)
                   || Implements(typeof(ICollection<>), type);
        }

        public static bool InheritsFrom<TSuper>(Type type)
        {
            var super = typeof(Type);

            return InheritsFrom(super, type);
        }

        public static bool InheritsFrom(Type super, Type driven)
        {
            if (super.IsInterface)
            {
                return Implements(super, driven);
            }

            return Extends(super, driven);
        }

        public static bool Implements<TInterface>(Type type)
        {
            var parent = type;

            var ifaceType = typeof(TInterface);

            return Implements(ifaceType, type);
        }

        public static bool Implements(Type ifaceType, Type type)
        {
            var parent = type;

            Func<Type, bool> predicate;

            if (ifaceType.IsGenericType)
            {
                predicate = t => t.IsGenericType && t.GetGenericTypeDefinition() == ifaceType;
            }
            else
            {
                predicate = t => t == ifaceType;
            }

            if (predicate(type))
            {
                return true;
            }

            while (parent != null)
            {
                var allInterfaces = type.GetInterfaces();

                foreach (var i in allInterfaces)
                {
                    if (predicate(i))
                    {
                        return true;
                    }
                }

                parent = parent.DeclaringType;
            }

            return false;
        }

        public static bool Extends<TSuper>(Type type)
        {
            var tBase = typeof(TSuper);

            return Extends(tBase, type);
        }

        public static bool Extends(Type tBase, Type type)
        {
            if (type == @tBase) return true;

            var parent = type.BaseType;

            if (parent != null)
            {
                return Extends(parent, @tBase);
            }

            return false;
        }

        public static bool IsReferenceType(Type t)
        {
            return !t.IsPrimitive &&
                   !t.IsValueType &&
                   t != typeof(string) &&
                   t != typeof(char);
        }

        /// <summary>
        /// Checks if the given type is effectively is a primitive, Defining <i>Effectively primitive</i> to be 
        /// a primitive type or <code>char</code> or <code>string</code>
        /// </summary>
        /// <typeparam name="T">Type to be checked.</typeparam>
        /// <returns>
        /// True if type is a primitive or <code>char</code> or <code>string</code>. Otherwise returns False
        /// </returns>
        public static bool IsEffectivelyPrimitive<T>()
        {
            return IsEffectivelyPrimitive(typeof(T));
        }

        /// <summary>
        /// Checks if the given type is effectively is a primitive, Defining <i>Effectively primitive</i> to be
        /// a primitive type or <code>char</code> or <code>string</code>
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>
        /// True if type is a primitive or <code>char</code> or <code>string</code>. Otherwise returns False
        /// </returns>
        public static bool IsEffectivelyPrimitive(Type type)
        {
            return !IsReferenceType(type);
        }

        public static List<Type> EnumerateEntities(Type type)
        {
            var result = new List<Type>();

            EnumerateEntities(type, result);

            return result;
        }

        private static void EnumerateEntities(Type type, List<Type> result)
        {
            if (IsReferenceType(type))
            {
                if (IsCollection(type))
                {
                    type = type.GenericTypeArguments[0];
                }

                result.Add(type);
            }

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                EnumerateEntities(pType, result);
            }
        }


        public static bool HasCyclicReferencedDescendants(Type type)
        {
            var alreadySeens = new List<Type>();

            return HasCyclicReferencedDescendants(type, alreadySeens);
        }

        private static bool HasCyclicReferencedDescendants(Type type, List<Type> alreadySeens)
        {
            if (!IsReferenceType(type))
            {
                return false;
            }

            foreach (var seen in alreadySeens)
            {
                if (type == seen)
                {
                    return true;
                }
            }

            alreadySeens.Add(type);

            IEnumerable<Type> children;

            if (IsCollection(type))
            {
                children = new Type[] { GetElementType(type) };
            }
            else
            {
                children = type.GetProperties().Select(p => p.PropertyType);
            }

            foreach (var child in children)
            {
                if (HasCyclicReferencedDescendants(child, alreadySeens))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsModel(Type type)
        {
            if (IsModelCache.ContainsKey(type))
            {
                return IsModelCache[type];
            }

            var isModel = IsModelNoneCached(type);

            IsModelCache.Add(type, isModel);

            return isModel;
        }

        private static bool IsModelNoneCached(Type type)
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
            {
                return false;
            }

            if (!IsReferenceType(type))
            {
                return false;
            }

            var properties = type.GetProperties();

            if (properties.Length == 0)
            {
                return false;
            }

            if (HasCyclicReferencedDescendants(type))
            {
                return false;
            }

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    return false;
                }

                var pType = property.PropertyType;

                if (IsReferenceType(pType) && !IsModel(pType) && !IsCollection(pType))
                {
                    Console.WriteLine($"Reference Rejected: {type.Name} because of {pType.Name}");

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if given type is any kind of collection, array and etc. Then returns the type of elements of it.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>The type of elements of collection/array or null if the type is not any kind of collection.</returns>
        public static Type GetElementType(Type type)
        {
            if (IsCollection(type))
            {
                if (type.IsArray)
                {
                    return type.GetElementType();
                }
                else if (type.GenericTypeArguments.Length > 0)
                {
                    return type.GenericTypeArguments[0];
                }

                return typeof(object);
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the given type, is one of the integral c# builtin types regarding this reference:
        /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
        /// (sbyte,byte ,short,ushort,int,uint,long,ulong ,nint,nuint)
        /// </summary>
        /// <typeparam name="T">Type to be checked</typeparam>
        /// <returns>True if T is an integer holding type. False otherwise</returns>
        public static bool IsIntegral<T>()
        {
            return IsIntegral(typeof(T));
        }

        /// <summary>
        /// Checks to see if the given type, is one of the integral c# builtin types regarding this reference:
        /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
        /// (sbyte,byte ,short,ushort,int,uint,long,ulong ,nint,nuint)
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <returns>True if "type", is an integer holding type. False otherwise</returns>
        public static bool IsIntegral(Type type)
        {
            return type == typeof(sbyte) ||
                   type == typeof(byte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong);
        }

        /// <summary>
        /// Checks to see if the given type, is one of the non-integral numbers in c#.
        /// (float,double ,decimal)
        /// </summary>
        /// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types"/>
        /// <typeparam name="T">Type to be checked</typeparam>
        /// <returns>True if "type", is a non-integer number holding type. False otherwise</returns>
        public static bool IsNonIntegerNumber<T>()
        {
            return IsNonIntegerNumber(typeof(T));
        }

        /// <summary>
        /// Checks to see if the given type, is one of the non-integral numbers in c#.
        /// (float,double ,decimal)
        /// </summary>
        /// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types"/>
        /// <param name="type">Type to be checked</param>
        /// <returns>True if "type", is a non-integer number holding type. False otherwise</returns>
        public static bool IsNonIntegerNumber(Type type)
        {
            return type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }

        /// <summary>
        /// Checks to see if the type represents a number.
        /// </summary>
        /// <typeparam name="T">The type to be checked.</typeparam>
        /// <returns>
        /// True if the type is one of the Integral or non-integral types in c#. Returns False otherwise.
        /// </returns>
        public static bool IsNumerical<T>()
        {
            return IsNumerical(typeof(T));
        }

        /// <summary>
        /// Checks to see if the type represents a number.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>
        /// True if the type is one of the Integral or non-integral types in c#. Returns False otherwise.
        /// </returns>
        public static bool IsNumerical(Type type)
        {
            return IsIntegral(type) || IsNonIntegerNumber(type);
        }

        /// <summary>
        /// Checks if type in question, is somehow an implementation or derivation of the given generic type 
        /// </summary>
        /// <param name="specific">The type in question.</param>
        /// <typeparam name="TGeneric">The generic type.</typeparam>
        /// <returns>True if specific type is a driven type of the generic type. Otherwise returns false.</returns>
        public static bool IsSpecificOf<TGeneric>(Type specific)
        {
            return IsSpecificOf(specific, typeof(TGeneric));
        }

        /// <summary>
        /// Checks if type in question, is somehow an implementation or derivation of the given generic type 
        /// </summary>
        /// <param name="specific">The type in question.</param>
        /// <param name="generic">The generic type.</param>
        /// <returns>True if specific type is a driven type of the generic type. Otherwise returns false.</returns>
        public static bool IsSpecificOf(Type specific, Type generic)
        {
            return specific != null &&
                   (
                       (specific.IsGenericType && specific.GetGenericTypeDefinition() == generic)
                       ||
                       IsSpecificOf(specific.BaseType, generic)
                   );
        }
    }
}