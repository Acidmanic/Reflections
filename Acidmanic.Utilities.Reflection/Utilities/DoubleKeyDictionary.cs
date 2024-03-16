using System.Collections.Generic;

namespace Acidmanic.Utilities.Reflection.Utilities
{
    public class DoubleKeyDictionary<TKey1, TKey2, TValue>
    {
        private Dictionary<TKey1, Dictionary<TKey2, TValue>> Data { get; }
        private readonly List<TKey1> _key1s;
        private readonly List<TKey2> _key2s;
        private readonly List<TValue> _values;


        public IReadOnlyList<TKey1> Key1S => _key1s;
        public IReadOnlyList<TKey2> Key2S => _key2s;
        public IReadOnlyList<TValue> Values => _values;

        public DoubleKeyDictionary()
        {
            Data = new Dictionary<TKey1, Dictionary<TKey2, TValue>>();
            _key1s = new List<TKey1>();
            _key2s = new List<TKey2>();
            _values = new List<TValue>();
        }


        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            if (Data.ContainsKey(key1))
            {
                return Data[key1].ContainsKey(key2);
            }

            return false;
        }


        public TValue this[TKey1 k1, TKey2 k2]
        {
            get => Data[k1][k2];
            set => Data[k1][k2] = value;
        }

        public void Add(TKey1 k1, TKey2 k2, TValue value)
        {
            if (!Data.ContainsKey(k1))
            {
                Data.Add(k1, new Dictionary<TKey2, TValue>());
            }

            Data[k1].Add(k2, value);

            _key1s.Add(k1);
            _key2s.Add(k2);
            _values.Add(value);
        }

        public void Remove(TKey1 k1, TKey2 k2, TValue value)
        {
            Data[k1].Remove(k2);
            _key1s.Remove(k1);
            _key2s.Remove(k2);
            _values.Remove(value);
        }
    }
}