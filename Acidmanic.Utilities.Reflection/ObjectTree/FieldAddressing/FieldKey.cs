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
    }
}