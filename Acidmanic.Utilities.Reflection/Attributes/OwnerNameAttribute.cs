using System;

namespace Acidmanic.Utilities.Reflection.Attributes
{
    public class OwnerNameAttribute:Attribute
    {
        public string TableName { get; }

        public OwnerNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}