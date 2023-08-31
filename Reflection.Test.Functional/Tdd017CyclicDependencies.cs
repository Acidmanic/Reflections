using System;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Reflection.Test.Functional.TestCaseModels;

namespace Reflection.Test.Functional
{
    public class Tdd017CyclicDependencies:TestBase
    {


        private class Foo
        {
            public int Id { get; set; }
            
            [TreatAsLeaf]
            public TimeStamp TimeStamp { get; set; }
        }
        
        public override void Main()
        {
            var hasCyclic = TypeCheck.HasCyclicReferencedDescendants(typeof(Foo),true);

            var isModel = TypeCheck.IsModel(typeof(Foo),true);
            
            Console.WriteLine($"Is Model: {isModel}, Has Cyclic: {hasCyclic}");
        }
    }
}