using System;
using System.Collections;
using System.Collections.Generic;

namespace Acidmanic.Utilities.Reflection.Sets
{
    public class ListWrap : IList<object>
    {
        private readonly object _rootObject;
        private readonly Type _listType;


        private readonly Action<object> _add = o => { };
        private readonly Action _clear = () => { };
        private readonly Func<object, bool> _contains = o => false;
        private readonly Action<object[], int> _copyTo = (o, i) => { };
        private readonly Action<object> _remove = o => { };
        private readonly Action<int> _removeAt = i => { };
        private readonly Func<int> _count = () => 0;
        private readonly Func<bool> _readOnly = () => false;
        private readonly Func<object, int> _indexOf = o => -1;
        private readonly Action<int, object> _insert = (o, i) => { };

        private readonly Func<int, object> _get = i => null;
        private readonly Action<int, object> _set = (i, o) => { };

        private readonly Func<IEnumerator<object>> _enumeratorFactory = () => null;

        public ListWrap(Type listType)
            : this(new ObjectInstantiator().CreateObject(listType, true), TypeCheck.GetElementType(listType))
        {
        }

        public ListWrap(object listObject) : this(listObject, TypeCheck.GetElementType(listObject.GetType()))
        {
        }


        /// <summary>
        /// Main Entry
        /// </summary>
        private ListWrap(object rootObject, Type elementType)
        {
            _rootObject = rootObject;
            ElementType = elementType;
            _listType = rootObject.GetType();

            if (_rootObject is IList list)
            {
                _add = o =>
                {
                    try
                    {
                        list.Add(o);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Your collection is not currently supported to be modified using" +
                                            " Reflection library. If it's an array, please consider using a List<> instead.");
                    }
                };
                _clear = list.Clear;
                _contains = list.Contains;
                _copyTo = list.CopyTo;
                _remove = list.Remove;
                _removeAt = list.RemoveAt;
                _count = () => list.Count;
                _readOnly = () => list.IsReadOnly;
                _indexOf = list.IndexOf;
                _insert = list.Insert;
                _get = index => list[index];
                _set = (index, value) => list[index] = value;
                _enumeratorFactory = () => new ObjectEnumerator(list.GetEnumerator());
            }
        }


        public IEnumerator<object> GetEnumerator()
        {
            return _enumeratorFactory();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(object item)
        {
            _add(item);
        }

        public void Clear()
        {
            _clear();
        }

        public bool Contains(object item)
        {
            return _contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            _copyTo(array, arrayIndex);
        }

        public bool Remove(object item)
        {
            int count = _count();

            _remove(item);

            return _count() == count - 1;
        }

        public int Count => _count();

        public bool IsReadOnly => _readOnly();

        public int IndexOf(object item)
        {
            return _indexOf(item);
        }

        public void Insert(int index, object item)
        {
            _insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _removeAt(index);
        }

        public object this[int index]
        {
            get => _get(index);
            set => _set(index, value);
        }

        public object RootObject => _rootObject;

        public Type ElementType { get; }
    }
}