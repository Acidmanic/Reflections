using System.Collections.Generic;

namespace Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing
{
    public class FieldKey : List<Segment>
    {
        public override string ToString()
        {
            var address = "";
            var sep = "";

            foreach (var segment in this)
            {
                address += sep + segment.ToString();
                sep = ".";
            }

            return address;
        }


        public static FieldKey Parse(string address)
        {
            var parts = address.Split(".");

            var result = new FieldKey();

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    return null;
                }

                var segmentString = part.Trim();

                if (string.IsNullOrEmpty(segmentString))
                {
                    return null;
                }

                var segment = Segment.Parse(segmentString);

                if (segment == null)
                {
                    return null;
                }

                result.Add(segment);
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is FieldKey fieldKey)
            {
                return Equals(fieldKey, FieldKeyComparisons.Strict);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public bool Equals(FieldKey obj)
        {
            return Equals(obj, FieldKeyComparisons.Strict);
        }
        
        public bool Equals(FieldKey obj,FieldKeyComparisons comparison)
        {
            if (obj == null)
            {
                return false;
            }
            if (Count != obj.Count)
            {
                return false;
            }

            var considerIndexesInTheWay = comparison == FieldKeyComparisons.Strict 
                                          || comparison == FieldKeyComparisons.IgnoreLastIndex;
            var considerLastIndex = comparison == FieldKeyComparisons.Strict;

            var lastIndex = Count - 1;
            
            for (int i = 0; i < lastIndex - 1; i++)
            {
                var s1 = this[i];
                var s2 = obj[i];

                if (!s1.Equals(s2,!considerIndexesInTheWay))
                {
                    return false;
                }
            }

            return this[lastIndex].Equals(obj[lastIndex], considerLastIndex);
        }

        public bool EqualsIgnoreIndex(FieldKey value)
        {
            return Equals(value, FieldKeyComparisons.IgnoreAllIndexes);
        }
    }
}