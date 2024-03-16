using System;

namespace Acidmanic.Utilities.Reflection.Casting
{
    public interface ICast
    {
        Type SourceType { get; }
        
        Type TargetType { get; }
        
        object Cast(object value);
    }
}