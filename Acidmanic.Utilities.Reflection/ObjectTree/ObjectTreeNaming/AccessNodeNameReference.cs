using System;
using System.Reflection;

namespace Acidmanic.Utilities.Reflection.ObjectTree.ObjectTreeNaming
{
    public class AccessNodeNameReference
    {
        public string GetNameForRoot(Type type)
        {
            return type.Name;
        }

        public string GetNameForCollectable(Type type)
        {
            return type.Name + "[-1]";
        }

        public string GetNameForCollectable(Type type, int index)
        {
            return type.Name + $"[{index}]";
        }

        public string GetNameForField(PropertyInfo propertyInfo)
        {
            return FieldInfo.GetMappedName(propertyInfo);
        }
    }
}