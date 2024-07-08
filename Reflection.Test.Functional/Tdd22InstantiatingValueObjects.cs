using System;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Reflection.Test.Functional;

public class Tdd22InstantiatingValueObjects : TestBase
{
    
    [AlteredType(typeof(long))]
    
    private struct Id
    {
        public long Value { get; }

        public Id(long value)
        {
            if (value < 10 || value > 1000)
            {
                throw new Exception("Cant Do That");
            }
            Value = value;
        }

        public static implicit operator long(Id value) => value.Value;
        public static implicit operator Id(long value) => new Id(value);
    }
    
    [AlteredType(typeof(long))]
    
    private class Id2
    {
        public long Value { get; }

        public Id2(long value)
        {
            if (value < 10 || value > 1000)
            {
                throw new Exception("Cant Do That");
            }
            Value = value;
        }

        public static implicit operator long(Id2 value) => value.Value;
        public static implicit operator Id2(long value) => new Id2(value);
    }
    
    private class Id3
    {
        public long Value { get; }

        public Id3(long value)
        {
            Value = value;
        }

        public static implicit operator long(Id3 value) => value.Value;
        public static implicit operator Id3(long value) => new Id3(value);
    }


    private class Model<TId>
    {
        public TId Id { get; set; }

        public string Name { get; set; }
    }

    public override void Main()
    {

        var oi = new ObjectInstantiator();

        var value1 = oi.CreateObject(typeof(Model<Id>), true);
        var value2 = oi.CreateObject(typeof(Model<Id2>), true);
        var value3 = oi.CreateObject(typeof(Model<Id3>), true);
        
        
    }
}