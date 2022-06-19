using System;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Reflection.Test.Functional
{
    public class Tdd001FieldKeyComparison:TestBase
    {
        public override void Main()
        {



            var key1 = FieldKey.Parse("Mani.Email.Char[0].Pixel[12]");
            var key2 = FieldKey.Parse("Mani.Email.Char[2].Pixel[27]");

            // Console.WriteLine($"{FieldKeyComparisons.Strict}-ly equal: {key1.Equals(key2,FieldKeyComparisons.Strict)}");
            // Console.WriteLine($"{FieldKeyComparisons.IgnoreAllIndexes}-ly equal: {key1.Equals(key2,FieldKeyComparisons.IgnoreAllIndexes)}");
            Console.WriteLine($"{FieldKeyComparisons.IgnoreLastIndex}-ly equal: {key1.Equals(key2,FieldKeyComparisons.IgnoreLastIndex)}");

            var seg1 = Segment.Parse("Mani[1]");
            var seg2 = Segment.Parse("Mani[2]");
            
            PrintLine();
            
            Console.WriteLine($"IgnoreIndex-ly equal: {seg1.Equals(seg2,true)}");
            Console.WriteLine($"Strictly equal: {seg1.Equals(seg2,false)}");
        }
    }
}