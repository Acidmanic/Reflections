using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

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
    }
}