using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Extensions;

namespace Reflection.Test.Functional
{
    public class Tdd007CloneCopyTest : TestBase
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
            
            public List<string> Pants { get; set; }
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
                    },
                    Pants =  new List<string>
                    {
                        "Jeans",
                        "Black"
                    }
                }
            };

            var aClone = a.Clone();
            
            PrintObject(aClone);
        }

    }
}