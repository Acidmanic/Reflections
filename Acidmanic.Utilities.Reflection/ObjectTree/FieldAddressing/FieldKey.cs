using System.Collections.Generic;

namespace Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing
{
    public class FieldKey : List<Segment>
    {
        public FieldKey()
        {
        }

        public FieldKey(IEnumerable<Segment> collection) : base(collection)
        {
        }

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

        public bool Equals(FieldKey obj, FieldKeyComparisons comparison)
        {
            if (obj == null)
            {
                return false;
            }

            if (Count != obj.Count)
            {
                return false;
            }

            if (Count == 0)
            {
                //Two empty keys
                return true;
            }
            var considerIndexesInTheWay = comparison == FieldKeyComparisons.Strict
                                          || comparison == FieldKeyComparisons.IgnoreLastIndex;
            var considerLastIndex = comparison == FieldKeyComparisons.Strict;

            var lastIndex = Count - 1;

            for (int i = 0; i < lastIndex ; i++)
            {
                var s1 = this[i];
                var s2 = obj[i];

                if (!s1.Equals(s2, !considerIndexesInTheWay))
                {
                    return false;
                }
            }

            return this[lastIndex].Equals(obj[lastIndex], !considerLastIndex);
        }

        public bool EqualsIgnoreIndex(FieldKey value)
        {
            return Equals(value, FieldKeyComparisons.IgnoreAllIndexes);
        }

        public Stack<int> GetIndexesStack()
        {
            Stack<int> indexes = new Stack<int>();

            for (int i = 0; i < this.Count; i++)
            {
                var segment = this[i];

                if (segment.Indexed)
                {
                    indexes.Push(segment.Index);
                }
            }

            return indexes;
        }

        public int[] GetIndexMap()
        {
            var map = new int[Count];
            
            for (int i = 0; i < this.Count; i++)
            {
                var segment = this[i];

                if (segment.Indexed)
                {
                    map[i] = segment.Index;
                }
                else
                {
                    map[i] = -1;
                }
            }

            return map;
        }

        public FieldKey Append(Segment segment)
        {
            var appended = new FieldKey(this); 

            appended.Add(segment);
            
            return appended;
        }

        public FieldKey UpLevel()
        {
            var upper = new FieldKey(this);
            
            if (Count > 0)
            {
                upper.RemoveAt(upper.Count - 1);
            }

            return upper;
        }

        public Segment TerminalSegment()
        {
            if (Count > 0)
            {
                return this[this.Count - 1];
            }

            return null;
        }
        
    }
}