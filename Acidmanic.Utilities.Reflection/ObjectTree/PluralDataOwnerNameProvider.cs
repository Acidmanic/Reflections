using System;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using CorePluralizer.Extensions;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class PluralDataOwnerNameProvider : IDataOwnerNameProvider
    {
        public string GetNameForOwnerType(Type ownerType)
        {
            var attributes = ownerType.GetCustomAttributes<OwnerNameAttribute>().ToList();

            if (attributes.Count > 0)
            {
                return attributes.Last().TableName;
            }

            var name = ownerType.Name;

            return name.ToPlural();
        }
    }
}