namespace Acidmanic.Utilities.Reflection.ObjectTree.StandardData
{
    internal class StandardConversionOptions:IStandardConversionOptionsBuilder
    {

        public bool DirectLeaves { get; private set; } = false;

        public bool ExcludeNullValues { get; private set; } = false;

        public bool CastAltered { get; private set; } = false;
        
        public IStandardConversionOptionsBuilder DirectLeavesOnly()
        {
            DirectLeaves = true;

            return this;
        }

        public IStandardConversionOptionsBuilder FullTree()
        {
            DirectLeaves = false;

            return this;
        }

        public IStandardConversionOptionsBuilder ExcludeNulls()
        {
            ExcludeNullValues = true;

            return this;
        }

        public IStandardConversionOptionsBuilder IncludeNulls()
        {
            ExcludeNullValues = false;

            return this;
        }

        public IStandardConversionOptionsBuilder UseAlternativeTypes()
        {
            CastAltered = true;

            return this;
        }

        public IStandardConversionOptionsBuilder UseOriginalTypes()
        {
            CastAltered = false;

            return this;
        }
    }
}