using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Utilities;

namespace Acidmanic.Utilities.Reflection.Casting
{
    public static class CastingDictionaryConstructionExtension
    {
        public static DoubleKeyDictionary<Type, Type, ICast> ToCastingDictionary(this IEnumerable<ICast> castings)
        {
            var castingDictionary = new DoubleKeyDictionary<Type, Type, ICast>();

            foreach (var cast in castings)
            {
                castingDictionary.Append(cast);
            }

            return castingDictionary;
        }

        public static void Append(this DoubleKeyDictionary<Type, Type, ICast> castingDictionary, ICast cast)
        {
            if (!castingDictionary.ContainsKey(cast.SourceType, cast.TargetType))
            {
                castingDictionary.Add(cast.SourceType, cast.TargetType, cast);
            }
        }
        
        public static void Remove(this DoubleKeyDictionary<Type, Type, ICast> castingDictionary, ICast cast)
        {
            if (castingDictionary.ContainsKey(cast.SourceType, cast.TargetType))
            {
                castingDictionary.Remove(cast.SourceType, cast.TargetType);
            }
        }
    }
}