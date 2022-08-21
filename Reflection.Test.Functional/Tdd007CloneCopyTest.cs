using System;
using System.Collections;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Reflection.Test.Functional
{
    public class Tdd007CloneCopyTest : TestBase
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
            
           
            var standardA = new ObjectEvaluator(a).ToStandardFlatData();

            standardA.ForEach(dp =>  Console.WriteLine(dp.Identifier + ": " + dp.Value));
            
            PrintLine();
            
            var standardAType = new ObjectEvaluator(typeof(A)).Map.Addresses;
            
            standardAType.ForEach(Console.WriteLine);
            
            PrintLine();
            
            var aClone = a.Clone();
            
            new ObjectEvaluator(aClone).ToStandardFlatData()
                .ForEach(point => Console.WriteLine(point.Identifier + ": " + point.Value));
        }

    }
}