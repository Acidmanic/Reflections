using System;

namespace Acidmanic.Utilities.Reflection.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class IgnoreNoneDataNodeAttribute:Attribute
{
    
}