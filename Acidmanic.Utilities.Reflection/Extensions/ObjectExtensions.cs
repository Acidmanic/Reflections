using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Acidmanic.Utilities.Reflection.Extensions
{
    public static  class ObjectExtensions
    {
        
        
        
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


        public static object Clone(this object value)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();
            
            var srcEvaluator = new ObjectEvaluator(value);

            var standardData = srcEvaluator.ToStandardFlatData();
            
            var dstEvaluator = new ObjectEvaluator(type);
            
            dstEvaluator.LoadStandardData(standardData);

            return dstEvaluator.RootObject;
        }
    }
}