using System;
using System.ComponentModel;

namespace Acidmanic.Utilities.Reflection.Casting
{
    public class ExplicitCast<TSource,TTarget>:ICast
    {
        public Type SourceType => typeof(TSource);

        public Type TargetType => typeof(TTarget);

        private readonly Func<TSource, TTarget> _convert;

        public ExplicitCast(Func<TSource, TTarget> convert)
        {
            _convert = convert;
        }

        public object Cast(object value)
        {
            return _convert((TSource)value);
        }
    }
}