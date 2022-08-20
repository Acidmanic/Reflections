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

            var standardData = evaluator.ToStandardFlatData();

            var selected = standardData.Where(dp =>
            {
                var node = evaluator.Map.NodeByAddress(dp.Identifier);


                return node!=null && node.IsLeaf && !node.IsRoot && node.Parent == evaluator.RootNode;
            });
            
            PrintObject(selected.ToList());
        }

    }
}