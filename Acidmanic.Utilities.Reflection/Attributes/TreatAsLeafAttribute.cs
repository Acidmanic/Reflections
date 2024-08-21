using System;

namespace Acidmanic.Utilities.Reflection.Attributes
{
    [AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Parameter)]
    public class TreatAsLeafAttribute:Attribute
    {

    }
}