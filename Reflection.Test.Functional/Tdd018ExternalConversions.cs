using System;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Reflection.Test.Functional.TestCaseModels;

namespace Reflection.Test.Functional
{
    public class Tdd018ExternalConversions : TestBase
    {
        private class P1
        {
            [TreatAsLeaf] public Guid Id { get; set; }
        }

        public override void Main()
        {
            var o = new P1() { Id = Guid.NewGuid() };

            var ev = new ObjectEvaluator(o);

            var standardData = ev.ToStandardFlatData();

            standardData[0].Value = o.Id.ToByteArray();

            var et = new ObjectEvaluator(typeof(P1));

            using (var s = CastScope.Create(new ExplicitCast<byte[], Guid>(b => new Guid(b))))
            {
                et.LoadStandardData(standardData);
            }

            var reo = et.RootObject as P1;

            Console.WriteLine($"Expected: {o.Id}");
            Console.WriteLine($"  Actual: {reo.Id}");
            
            var et2 = new ObjectEvaluator(typeof(P1));
            
            et2.LoadStandardData(standardData);
            var reo2 = et.RootObject as P1;
            
            Console.WriteLine($"Expected: {o.Id}");
            Console.WriteLine($"  Actual: {reo2.Id}");
        }
    }
}