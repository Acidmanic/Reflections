namespace Acidmanic.Utilities.Reflection.ObjectTree.Evaluators
{
    public interface IEvaluator
    {
        object Read(object parentObject);

        void Write(object parentObject, object value);
    }
}