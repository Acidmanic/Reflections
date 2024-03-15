using System;

namespace Acidmanic.Utilities.Reflection.Attributes
{
    [AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct)]
    public class AlteredTypeAttribute : Attribute
    {
        public AlteredTypeAttribute(Type alternativeType)
        {
            AlternativeType = alternativeType;
        }


        public Type AlternativeType { get; }
    }
}