using System;

namespace Acidmanic.Utilities.Reflection.Attributes
{
    public class MemberNameAttribute : Attribute
    {
        public string Name { get; }

        public MemberNameAttribute(string name)
        {
            Name = name;
        }
    }
}