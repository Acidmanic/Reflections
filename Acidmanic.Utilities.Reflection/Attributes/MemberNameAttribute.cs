using System;

namespace Acidmanic.Utilities.Reflection.Attributes
{
    [AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Parameter)]
    public class MemberNameAttribute : Attribute
    {
        public string Name { get; }

        public MemberNameAttribute(string name)
        {
            Name = name;
        }
    }
}