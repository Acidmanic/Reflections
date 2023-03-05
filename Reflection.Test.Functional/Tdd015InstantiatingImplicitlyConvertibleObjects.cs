using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Litbid.DataAccess.Models;
using Reflection.Test.Functional.TestCaseModels;

namespace Reflection.Test.Functional
{
 
    public class Tdd015InstantiatingImplicitlyConvertibleObjects : TestBase
    {
        
        class NotThatBadObject
        {
            public long Id { get; set; }
            
            [TreatAsLeaf]
            public TimeStamp BadProperty { get; set; }
        }
        
        public override void Main()
        {

             var instance = new ObjectInstantiator().CreateObject<NotThatBadObject>(true);
            //var instance = new ObjectInstantiator().BlindInstantiate(typeof(TimeStamp));


        }
    }
}