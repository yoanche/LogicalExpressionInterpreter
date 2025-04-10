using KR_SAA.Models;
using System;


namespace KR_SAA.DataStructures
{
    public class MyHashTable<TKey, TValue>
    {
        private readonly MyList<MyKeyValuePair<TKey, TValue>>[] _buckets;
        private readonly int _capacity;

        public MyHashTable(int capacity)
        {
            _capacity = capacity;
            _buckets = new MyList<MyKeyValuePair<TKey, TValue>>[capacity];

            for (int i = 0; i < _capacity; i++)
            {
                _buckets[i] = new MyList<MyKeyValuePair<TKey, TValue>>(capacity);
            }
        }

        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode();
            return (hash & 0x7FFFFFFF) % _capacity; 
        }

        public void Add(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);

            for (int i = 0; i < _buckets[index].Count; i++)
            {
                if (_buckets[index][i].GetKey().Equals(key))
                {
                    throw new InvalidOperationException($"Key '{key}' already exists.");
                }
            }

            _buckets[index].Add(new MyKeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            int index = GetBucketIndex(key);

            for (int i = 0; i < _buckets[index].Count; i++)
            {
                if (_buckets[index][i].GetKey().Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        public TValue GetValue(TKey key)
        {
            int index = GetBucketIndex(key);

            for (int i = 0; i < _buckets[index].Count; i++)
            {
                if (_buckets[index][i].GetKey().Equals(key))
                {
                    return _buckets[index][i].GetValue();
                }
            }

            throw new Exception($"Key '{key}' not found.");
        }
    }

    public class MyKeyValuePair<TKey, TValue>
    {
        private readonly TKey _key;
        private readonly TValue _value;

        public MyKeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        public TKey GetKey() => _key;

        public TValue GetValue() => _value;
    }
}
