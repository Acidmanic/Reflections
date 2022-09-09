using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional
{
    public class Tdd010TreatAsLeafTest:TestBase
    {

        class A
        {
            public int Id { get; set; }
            
            public B B { get; set; }
            
            public List<C> Cees { get; set; }
        }

        class B
        {
            public int Id { get; set; }
            
            [TreatAsLeaf]
            public List<string> Coats { get; set; }
            
            public List<string> Pants { get; set; }
        }


        class C
        {
            public string Name { get; set; }

            public override string ToString()
            {
                return "C: "+Name;
            }
        }
        public override void Main()
        {
            var a = new A
            {
                Id = 1,
                B = new B
                {
                    Id = 2,
                    Coats = new List<string>
                    {
                        "WhiteCoat",
                        "GrayCoat"
                    },
                    Pants =  new List<string>
                    {
                        "Jeans",
                        "Black"
                    }
                },
                Cees = new List<C>
                {
                    new C{Name = "FIRST"},
                    new C{Name = "SECOND"},
                    new C{Name = "THIRD"},
                }
            };



            var node = ObjectStructure.CreateStructure<A>(true);

            var evaluator = new ObjectEvaluator(a);
            
            var standardData = evaluator.ToStandardFlatData();
            
            standardData.ForEach( d => Console.WriteLine(d.Identifier + ": " + d.Value));


        }
    }
}