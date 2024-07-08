using System;

namespace Acidmanic.Utilities.Reflection.Attributes;

public class DefaultAttribute:Attribute
{
    public object Default { get; }


    public DefaultAttribute(int value)
    {
        Default = value;
    }
    public DefaultAttribute(long value)
    {
        Default = value;
    }
    public DefaultAttribute(short value)
    {
        Default = value;
    }
    public DefaultAttribute(double value)
    {
        Default = value;
    }
    public DefaultAttribute(float value)
    {
        Default = value;
    }
    public DefaultAttribute(string value)
    {
        Default = value;
    }
    public DefaultAttribute(char value)
    {
        Default = value;
    }
    public DefaultAttribute(decimal value)
    {
        Default = value;
    }
    public DefaultAttribute(byte value)
    {
        Default = value;
    }
    public DefaultAttribute(bool value)
    {
        Default = value;
    }
    public DefaultAttribute(ulong value)
    {
        Default = value;
    }
    public DefaultAttribute(uint value)
    {
        Default = value;
    }
    public DefaultAttribute(sbyte value)
    {
        Default = value;
    }
    public DefaultAttribute(nint value)
    {
        Default = value;
    }
    public DefaultAttribute(nuint value)
    {
        Default = value;
    }
    public DefaultAttribute(ushort value)
    {
        Default = value;
    }


    
}