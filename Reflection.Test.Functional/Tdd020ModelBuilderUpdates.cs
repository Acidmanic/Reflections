using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.Dynamics;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Newtonsoft.Json;
using Reflection.Test.Functional.TestCaseModels;

namespace Reflection.Test.Functional
{
    public class Tdd020ModelBuilderUpdates : TestBase
    {
        
        
        public override void Main()
        {
            var builder = new ModelBuilder("Mashgholam");

            builder.AddProperty("First", typeof(string),"Some value for first argument");
            builder.AddProperty("Second", typeof(bool),true);
            builder.AddProperty("Third", typeof(int),123456);
            builder.AddProperty("Fourth", typeof(Guid),Guid.NewGuid());

            

            var mash = builder.BuildObject();

            Console.WriteLine(JsonConvert.SerializeObject(mash));
            
        }
    }
}