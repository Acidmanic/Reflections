using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional
{
    public class Tdd009TestFieldInclusion:TestBase
    {

        private class Outer
        {
            public int Id { get; set; }
            
            public Middle MiddleItem { get; set; }
            
            public List<Center> Centers { get; set; } 
        }


        private class Middle
        {
            public string Name { get; set; }
            
            public Inner Inner { get; set; }
            
        }

        private class Inner
        {
            public string Name { get; set; }
            
        }

        private class Center
        {
            public int Id { get; set; }
        }
        
        public override void Main()
        {
            var fieldMarker = new FiledManipulationMarker();

            fieldMarker.Exclude((Outer o) => o.MiddleItem.Inner.Name);

            Console.WriteLine("MiddleItem.Inner.Name is included: " + fieldMarker.IsIncluded((Outer o) => o.MiddleItem.Inner.Name));
            Console.WriteLine("MiddleItem.Inner is included: " + fieldMarker.IsIncluded((Outer o) => o.MiddleItem.Inner));
            Console.WriteLine("MiddleItem.Name is included: " + fieldMarker.IsIncluded((Outer o) => o.MiddleItem.Name));
            
        }
    }
}