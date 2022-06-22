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
    }
}