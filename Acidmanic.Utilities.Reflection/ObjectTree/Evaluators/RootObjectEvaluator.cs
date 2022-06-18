namespace Acidmanic.Utilities.Reflection.ObjectTree.Evaluators
{
    public class RootObjectEvaluator:IEvaluator
    {
        
        
        
        public object Read(object parentObject)
        {
            return parentObject;
        }

        public void Write(object parentObject, object value)
        {
            //TODO: Log
            //Trying to rewrite the whole root object is not reasonable
        }
    }
}