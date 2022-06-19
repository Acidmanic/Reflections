using System;
using System.Numerics;

namespace Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing
{
    public class Segment
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public bool Indexed { get; }

        public Segment(string name)
        {
            Index = -1;

            Indexed = false;

            Name = name;
        }

        public Segment(string name, int index)
        {
            Index = index;

            Name = name;

            Indexed = true;
        }


        public override string ToString()
        {
            var qualifiedName = Name;

            if (Indexed)
            {
                qualifiedName += $"[{Index}]";
            }

            return qualifiedName;
        }

        public static Segment Parse(string segmentString)
        {
            segmentString = segmentString.Trim();

            var parts = segmentString.Split(new char[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return new Segment(parts[0]);
            }

            if (parts.Length == 2)
            {
                if (int.TryParse(parts[1], out var index))
                {
                    return new Segment(parts[0], index);
                }
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is Segment segment)
            {
                return Equals(segment, false);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public bool Equals(Segment obj)
        {
            return Equals(obj, false);
        }
        
        public bool Equals(Segment obj, bool ignoreIndex)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.Name != Name)
            {
                return false;
            }

            if (obj.Indexed != Indexed)
            {
                return false;
            }

            if (ignoreIndex)
            {
                return true;
            }

            return Index == obj.Index;
        }
    }
}