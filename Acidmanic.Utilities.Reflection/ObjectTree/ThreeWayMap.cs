using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class ThreeWayMap<TFirst, TSecond, TThird>
    {
        private readonly Dictionary<TFirst, int> _indexByFirst;
        private readonly Dictionary<TSecond, int> _indexBySecond;
        private readonly Dictionary<TThird, int> _indexByThird;

        protected readonly List<TFirst> Firsts;
        protected readonly List<TSecond> Seconds;
        protected readonly List<TThird> Thirds;

        public ThreeWayMap()
        {
            Thirds = new List<TThird>();
            Seconds = new List<TSecond>();
            Firsts = new List<TFirst>();
            _indexByFirst = new Dictionary<TFirst, int>();
            _indexBySecond = new Dictionary<TSecond, int>();
            _indexByThird = new Dictionary<TThird, int>();
        }

        public void Add(TFirst first, TSecond second, TThird third)
        {
            int index = Firsts.Count;

            Firsts.Add(first);
            Seconds.Add(second);
            Thirds.Add(third);

            _indexByFirst.Add(first, index);
            _indexBySecond.Add(second, index);
            _indexByThird.Add(third, index);
        }

        protected int IndexOf(TFirst key)
        {
            if (_indexByFirst.ContainsKey(key))
            {
                return _indexByFirst[key];
            }

            return -1;
        }

        protected int IndexOf(TSecond key)
        {
            if (_indexBySecond.ContainsKey(key))
            {
                return _indexBySecond[key];
            }

            return -1;
        }

        protected int IndexOf(TThird key)
        {
            if (_indexByThird.ContainsKey(key))
            {
                return _indexByThird[key];
            }

            return -1;
        }

        protected TFirst GetFirst(int index)
        {
            if (index >= 0 && index < Firsts.Count)
            {
                return Firsts[index];
            }

            throw new IndexOutOfRangeException();
        }

        protected TSecond GetSecond(int index)
        {
            if (index >= 0 && index < Firsts.Count)
            {
                return Seconds[index];
            }

            throw new IndexOutOfRangeException();
        }

        protected TThird GetThird(int index)
        {
            if (index >= 0 && index < Firsts.Count)
            {
                return Thirds[index];
            }

            throw new IndexOutOfRangeException();
        }

        public bool ContainsKey(TFirst key)
        {
            return _indexByFirst.ContainsKey(key);
        }

        public bool ContainsKey(TSecond key)
        {
            return _indexBySecond.ContainsKey(key);
        }

        public bool ContainsKey(TThird key)
        {
            return _indexByThird.ContainsKey(key);
        }
    }
}