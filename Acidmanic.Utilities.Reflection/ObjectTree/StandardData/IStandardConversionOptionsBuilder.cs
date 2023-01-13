namespace Acidmanic.Utilities.Reflection.ObjectTree.StandardData
{
    public interface IStandardConversionOptionsBuilder
    {

        IStandardConversionOptionsBuilder DirectLeavesOnly();
        
        IStandardConversionOptionsBuilder FullTree();
        
        IStandardConversionOptionsBuilder ExcludeNulls();
        
        IStandardConversionOptionsBuilder IncludeNulls();
        
        IStandardConversionOptionsBuilder UseAlternativeTypes();
        
        
        IStandardConversionOptionsBuilder UseOriginalTypes();
        
    }
}