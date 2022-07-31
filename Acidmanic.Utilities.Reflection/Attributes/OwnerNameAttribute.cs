using System;

namespace Acidmanic.Utilities.Reflection.Attributes
{
    [AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]  
    public class OwnerNameAttribute:Attribute
    {
        public string TableName { get; }

        public OwnerNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}