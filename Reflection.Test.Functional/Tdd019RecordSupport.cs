using System;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Reflection.Test.Functional.TestCaseModels;

namespace Reflection.Test.Functional
{
    public class Tdd019RecordSupport : TestBase
    {
        public sealed record R(string Name, string Surname, Guid Id);
        
        public override void Main()
        {
            var r = new R("Mani", "Moayedi", Guid.NewGuid());

            var ev = new ObjectEvaluator(r);

            var flatData = ev.ToStandardFlatData();

            var rev = new ObjectEvaluator(typeof(R));

            rev.LoadStandardData(flatData);

            var rer = rev.RootObject as R;

            Console.WriteLine($"Re Constructed: {rer.Name}, {rer.Surname}, {rer.Id}");
        }
    }
}