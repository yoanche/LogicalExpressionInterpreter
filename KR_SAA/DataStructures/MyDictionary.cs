using System;

namespace KR_SAA.DataStructures
{
    public class MyDictionary<TKey, TValue>
    {
        private TKey[] _keys;
        private TValue[] _values;
        private int _count;

        public MyDictionary(int capacity = 16)
        {
            _keys = new TKey[capacity];
            _values = new TValue[capacity];
            _count = 0;
        }

        public void Add(TKey key, TValue value)
        {
           
            for (int i = 0; i < _count; i++)
            {
                if (_keys[i].Equals(key))
                    throw new InvalidOperationException($"Key {key} already exists.");
            }

            _keys[_count] = key;
            _values[_count] = value;
            _count++;
        }

        public TValue Get(TKey key)
        {
            for (int i = 0; i < _count; i++)
                if (_keys[i].Equals(key))
                    return _values[i];

            throw new KeyNotFoundException($"Key {key} not found.");
        }

        public bool ContainsKey(TKey key)
        {
            for (int i = 0; i < _count; i++)
                if (_keys[i].Equals(key))
                    return true;

            return false;
        }

    }
}
