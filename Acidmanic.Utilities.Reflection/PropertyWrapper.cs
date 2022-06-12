using System;
using System.Reflection;

namespace Acidmanic.Utilities.Reflection
{

    public class PropertyWrapper:PropertyWrapper<object>
    {

    }

    public class PropertyWrapper<T>
    {


        private Func<T> _read = () => default;
        private Action<T> _write = (value) => { };



        public T Value
        {
            get
            {
                return _read();
            }
            set
            {
                _write(value);
            }
        }

        public PropertyWrapper()
        {

        }

        public PropertyWrapper(PropertyInfo propertyInfo, object obj)
        {
            if (propertyInfo.CanRead)
            {
                _read = () =>
                {
                    try
                    {
                        return (T) propertyInfo.GetValue(obj);
                    }
                    catch (Exception) { }
                    return default;
                };
            }
            if (propertyInfo.CanWrite)
            {
                _write = (value) =>
                {
                    try
                    {
                        propertyInfo.SetValue(obj,(object) value);
                    }
                    catch (Exception) { }
                };
            }
        }

        public PropertyWrapper(Func<T> read, Action<T> write)
        {
            _read = read;

            _write = write;
        }

    }
}
