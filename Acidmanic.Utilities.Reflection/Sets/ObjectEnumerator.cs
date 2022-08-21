using System.Collections;
using System.Collections.Generic;

namespace Acidmanic.Utilities.Reflection.Sets
{
    internal class ObjectEnumerator : IEnumerator<object>
    {
        private readonly IEnumerator _enumerator;

        public ObjectEnumerator(ICollection collection)
        {
            _enumerator = collection.GetEnumerator();
        }

        public ObjectEnumerator(IEnumerator enumerator)
        {
            _enumerator = enumerator;
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
}