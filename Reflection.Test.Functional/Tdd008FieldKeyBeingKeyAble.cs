using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional
{
    public class Tdd008FieldKeyBeingKeyAble:TestBase
    {
        public override void Main()
        {
            
            var key = new FieldKey().Append(new Segment("A")).Append(new Segment("B"));
            var key2 = new FieldKey().Append(new Segment("A")).Append(new Segment("B"));
            
            
            var dic = new Dictionary<FieldKey,string>();
            
            dic.Add(key,"first");

            if (dic.ContainsKey(key2))
            {
                Console.WriteLine("Already exists");
            }
            else
            {
                dic.Add(key2,"Seconds");
            }
        }
    }
}