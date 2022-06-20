using System.Collections.Generic;

namespace Acidmanic.Utilities.Reflection.ObjectTree.StandardData
{
    public class Record : List<DataPoint>
    {
        public Record()
        {
        }

        public Record(IEnumerable<DataPoint> collection) : base(collection)
        {
        }

        public void Add(string identifier, object value)
        {
            Add(new DataPoint
            {
                Identifier = identifier,
                Value = value
            });
        }
    }
}