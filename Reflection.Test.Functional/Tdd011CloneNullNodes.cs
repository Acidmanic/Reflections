using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional
{
    public class Tdd011CloneNullNodes:TestBase
    {

        class OuterClass
        {
            public int Primitive { get; set; }
            
            public InnerClass ValuedReference { get; set; }
            
            public InnerClass NullReference { get; set; }
        }

        public class InnerClass
        {
            public int Id { get; set; }
            
            public string Name { get; set; }
        }
        
        public override void Main()
        {
            var source = new OuterClass
            {
                Primitive = 1,
                ValuedReference = new InnerClass
                {
                    Id = 2,
                    Name = "Three"
                },
                NullReference = null
            };

            var clone = source.Clone();
            
            
        }
    }
}

