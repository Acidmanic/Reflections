using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Acidmanic.Utilities.Reflection.Utilities;

namespace Acidmanic.Utilities.Reflection.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Compares two objects using object.Equals() but checks for being equally/un-equally null before that. 
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns>True if objects are equal, False if not.</returns>
        public static bool AreEqualAsNullables(this object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }

            if (o1 == null || o2 == null)
            {
                return false;
            }

            return o1.Equals(o2);
        }

        /// <summary>
        /// Creates a new instance of an object with all it's properties valued regarding the source object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>a copy of given object.</returns>
        public static object Clone(this object value)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();

            var srcEvaluator = new ObjectEvaluator(value);

            var standardData = srcEvaluator.ToStandardFlatData(excludeNulls: true);

            var dstEvaluator = new ObjectEvaluator(new ObjectInstantiator().BlindInstantiate(type));

            dstEvaluator.LoadStandardData(standardData);

            return dstEvaluator.RootObject;
        }

        /// <summary>
        /// First check if both or one of the objects are null, if both not null then checks all properties of both
        /// objects to be equal recursively.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns>True if objects are equal, False if not.</returns>
        public static bool AreEquivalentsWith(this object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }

            if (o1 == null || o2 == null)
            {
                return false;
            }

            var standard1 = new ObjectEvaluator(o1).ToStandardFlatData();
            var standard2 = new ObjectEvaluator(o2).ToStandardFlatData();

            if (standard1.Count != standard2.Count)
            {
                return false;
            }

            for (int i = 0; i < standard1.Count; i++)
            {
                if (standard1[i].Identifier != standard2[i].Identifier)
                {
                    return false;
                }

                if (!standard1[i].Value.AreEqualAsNullables(standard2[i].Value))
                {
                    return false;
                }
            }

            return true;
        }

        public static void CopyInto(this object me, object target)
        {
            var myEvaluator = new ObjectEvaluator(me);
            var tarEvaluator = new ObjectEvaluator(target);


            var myData = myEvaluator.ToStandardFlatData();

            tarEvaluator.LoadStandardData(myData);
        }


        public static void CopyInto(this object me, object target, params string[] excludedAddresses)
        {
            var myEvaluator = new ObjectEvaluator(me);
            var tarEvaluator = new ObjectEvaluator(target);


            var myData = myEvaluator.ToStandardFlatData();

            var filtered = myData.Where(dp => !excludedAddresses.Contains(dp.Identifier));

            var srcData = new Record(filtered);


            tarEvaluator.LoadStandardData(srcData);
        }

        /// <summary>
        /// This method tries to make sure the returning value is assignable to given target type.
        ///  If it's somehow inherits from the given type, it will return it without any change. And if
        /// it's not inherited in any way, then it will try to cast it. 
        /// </summary>
        /// <param name="value">value to be casted</param>
        /// <param name="targetType">the type of assignee variable</param>
        /// <param name="castings">A Dictionary Of ICast Objects to be used for casting into different types</param>
        /// <returns>casted object</returns>
        public static object CastTo(this object value, Type targetType,
            params ICast[] castings)
        {
            return CastTo(value, targetType, castings.ToCastingDictionary());
        }

        /// <summary>
        /// This method tries to make sure the returning value is assignable to given target type.
        ///  If it's somehow inherits from the given type, it will return it without any change. And if
        /// it's not inherited in any way, then it will try to cast it. 
        /// </summary>
        /// <param name="value">value to be casted</param>
        /// <param name="targetType">the type of assignee variable</param>
        /// <param name="castings">A Dictionary Of ICast Objects to be used for casting into different types</param>
        /// <returns>casted object</returns>
        public static object CastTo(this object value, Type targetType,
            IEnumerable<ICast> castings)
        {
            DoubleKeyDictionary<Type, Type, ICast> castingDictionary = null;

            if (castings != null)
            {
                castingDictionary = castings.ToCastingDictionary();
            }

            return CastTo(value, targetType, castingDictionary);
        }

        /// <summary>
        /// This method tries to make sure the returning value is assignable to given target type.
        ///  If it's somehow inherits from the given type, it will return it without any change. And if
        /// it's not inherited in any way, then it will try to cast it. 
        /// </summary>
        /// <param name="value">value to be casted</param>
        /// <param name="targetType">the type of assignee variable</param>
        /// <param name="castings">A Dictionary Of ICast Objects to be used for casting into different types</param>
        /// <returns>casted object</returns>
        public static object CastTo(this object value, Type targetType,
            DoubleKeyDictionary<Type, Type, ICast> castings = null)
        {
            if (value == null)
            {
                return null;
            }

            var sourceType = value.GetType();

            castings ??= new DoubleKeyDictionary<Type, Type, ICast>();

            if (castings.ContainsKey(sourceType, targetType))
            {
                return castings[sourceType, targetType].Cast(value);
            }

            if (targetType.IsAssignableFrom(sourceType))
            {
                return value;
            }

            var declaredConversions = TypeCheck.GetIxplicitOperatorMethods
                (value.GetType(), targetType).ToList();

            if (declaredConversions.Count > 0)
            {
                return declaredConversions[0].Invoke(null, new[] { value });
            }

            if (targetType.IsEnum) return Enum.ToObject(targetType, value);

            var forceCasted = Convert.ChangeType(value, targetType);

            return forceCasted;
        }

        public static double AsNumber(this object value, double defaultValue = 0)
        {
            if (value != null)
            {
                var type = value.GetType();

                if (TypeCheck.IsNumerical(type))
                {
                    var casted = value.CastTo(typeof(double));

                    if (casted != null)
                    {
                        return (double)casted;
                    }
                }
            }

            return defaultValue;
        }

        public static long AsIntegral(this object value, long defaultValue = 0)
        {
            if (value != null)
            {
                var type = value.GetType();

                if (TypeCheck.IsNumerical(type))
                {
                    var casted = value.CastTo(typeof(long));

                    if (casted != null)
                    {
                        return (long)casted;
                    }
                }
            }

            return defaultValue;
        }
    }
}