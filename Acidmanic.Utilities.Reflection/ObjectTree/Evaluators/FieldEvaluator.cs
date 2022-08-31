using System;
using System.Reflection;

namespace Acidmanic.Utilities.Reflection.ObjectTree.Evaluators
{
    public class FieldEvaluator : IEvaluator
    {
        private readonly PropertyInfo _propertyInfo;

        public FieldEvaluator(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }


        public object Read(object parentObject)
        {
            return _propertyInfo.GetValue(parentObject);
        }

        public void Write(object parentObject, object value)
        {
            var forceCasted = value==null?null: Convert.ChangeType(value, _propertyInfo.PropertyType);
            
            _propertyInfo.SetValue(parentObject, forceCasted);
        }
    }
}