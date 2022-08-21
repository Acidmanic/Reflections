using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Reflection.Test.Functional.Models;

namespace Reflection.Test.Functional
{
    public class Tdd006DirectLeaveaOnly : TestBase
    {
        class A
        {
            public int Id { get; set; }

            public B B { get; set; }

            public string Name { get; set; }
        }

        class B
        {
            public int Id { get; set; }

            public string[] Coats { get; set; }
        }

        public override void Main()
        {
            var a = new A
            {
                Id = 1,
                Name = "Direct",
                B = new B
                {
                    Id = 2,
                    Coats = new string[]
                    {
                        "WhiteCoat",
                        "GrayCoat"
                    }
                }
            };

            var evaluator = new ObjectEvaluator(a);

            
            var selected = evaluator.GetStandardFlatDataForDirectLeaves();


            selected.ForEach(dp => Console.WriteLine(dp.ToString()));
        }
    }
}