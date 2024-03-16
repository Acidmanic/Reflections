using System;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Casting;
using Acidmanic.Utilities.Reflection.Extensions;

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
            var assignable = value.CastTo(_propertyInfo.PropertyType,CastScope.GetAvailableCasts());
            
            _propertyInfo.SetValue(parentObject, assignable);
        }

        
    }
}