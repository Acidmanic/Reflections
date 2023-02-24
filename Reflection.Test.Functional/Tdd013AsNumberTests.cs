using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Litbid.DataAccess.Models;

namespace Reflection.Test.Functional
{
    public class Tdd013AsNumberTests : TestBase
    {
      
        public override void Main()
        {


            var lValue = 100L;

            var casted = lValue.AsNumber();


            Console.WriteLine(casted);
        }
    }
}