using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Reflection.Test.Functional.Models;

namespace Reflection.Test.Functional
{
    public class Tdd004ForceCastTest : TestBase
    {

        class A
        {
            public int Id { get; set; }
        }
        
        public override void Main()
        {

            var dp = new DataPoint
            {
                Identifier = "A.Id",
                Value = 128L
            };
            
            var evaluator = new ObjectEvaluator(typeof(A));
            
            evaluator.Write(dp.Identifier,dp.Value);


            var result = evaluator.RootObject;
            
            PrintObject(result);
        }
    }
}