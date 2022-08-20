using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Acidmanic.Utilities.Reflection.Sets
{
    public class CollectionCollection : ICollection<object>
    {
        private readonly ICollection _collection;
        private readonly Type _elementType;

        private readonly MethodInfo _add;
        private readonly MethodInfo _clear;
        private readonly MethodInfo _contains;
        private readonly MethodInfo _remove;

        public CollectionCollection(Type collectionType) :
            this(collectionType.GetElementType(), (ICollection) new ObjectInstantiator().BlindInstantiate(collectionType))
        {
        }

        public CollectionCollection(ICollection collection) : this(TypeCheck.GetElementType(collection.GetType()), collection)
        {
        }

        private CollectionCollection(Type elementType, ICollection collection)
        {
            _elementType = elementType;

            var genericType = collection.GetType();

            _add = genericType.GetMethod("Add", new Type[] {elementType});
            _clear = genericType.GetMethod("Clear", new Type[] { });
            _contains = genericType.GetMethod("Contains", new Type[] {elementType});
            _remove = genericType.GetMethod("Remove", new Type[] {elementType});

            _collection = collection;
        }


        private class ObjectEnumerator : IEnumerator<object>
        {
            private readonly IEnumerator _enumerator;

            public ObjectEnumerator(ICollection collection)
            {
                _enumerator = collection.GetEnumerator();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public object Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return new ObjectEnumerator(_collection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(object item)
        {
            _add.Invoke(_collection, new object[] {item});
        }

        public void Clear()
        {
            _clear.Invoke(_collection, new object[] { });
        }

        public bool Contains(object item)
        {
            var contains = _contains.Invoke(_collection, new object[] {item});

            if (contains is bool doesContain)
            {
                return doesContain;
            }

            return false;
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int availableLength = array.Length - arrayIndex;

            if (availableLength < this.Count)
            {
                throw new ArgumentException("There is not enough room to copy the values into array.");
            }

            var index = arrayIndex;

            foreach (var item in _collection)
            {
                array[index] = item;

                index++;
            }
        }

        public bool Remove(object item)
        {
            var removed = _remove.Invoke(_collection, new object[] {item});

            if (removed is bool didRemove)
            {
                return didRemove;
            }

            return false;
        }

        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public Type ElementType => _elementType;
    }
}