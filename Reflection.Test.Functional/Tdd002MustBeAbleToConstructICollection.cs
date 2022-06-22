using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;

namespace Reflection.Test.Functional
{
    public class Tdd002MustBeAbleToConstructICollection:TestBase
    {
        
        public interface IDumb
        {
            string Name { get; set; }
            
            int Id { get; set; }
        }
        
        public class MyDumb:IDumb
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class BadModel
        {
            public long Id { get; set; }
            
            public int Count { get; set; }
            
            public List<IDumb> Dumbs { get; set; }
            
            public List<string> StupidNames { get; set; }
            
            public IDumb TheDumbest { get; set; }
        }
        public override void Main()
        {
            ICollection<int> list = (ICollection<int>) new ObjectInstantiator().BlindInstantiate(typeof(ICollection<int>)) ;
            
            BadModel badModel = new ObjectInstantiator().CreateObject(typeof(BadModel)) as BadModel;
            
            PrintObject(badModel);
        }
    }
}