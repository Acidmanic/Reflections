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


    private class Model
    {
        public Id Id { get; set; }

        public string Name { get; set; }
    }

    public override void Main()
    {

        var oi = new ObjectInstantiator();

        var value = oi.CreateObject(typeof(Model), true);
        

    }
}