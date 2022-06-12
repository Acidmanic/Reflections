using System;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public interface IDataOwnerNameProvider
    {
        string GetNameForOwnerType(Type ownerType);
    }
}