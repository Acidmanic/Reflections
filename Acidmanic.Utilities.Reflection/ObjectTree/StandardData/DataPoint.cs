namespace Acidmanic.Utilities.Reflection.ObjectTree.StandardData
{
    public class DataPoint
    {
        public string Identifier { get; set; }

        public object Value { get; set; }

        public override string ToString()
        {
            return $"{Identifier}: {Value}";
        }
    }
}