using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Acidmanic.Utilities.Reflection.Extensions
{
    public static class PropertyInfoAttributeExtensions
    {



        public static bool IsTreatAsALeaf(this PropertyInfo property)
        {
            var leafAt = property.GetCustomAttribute<TreatAsLeafAttribute>();

            if (leafAt != null)
            {
                return true;
            }

            return false;
        }
    }
}