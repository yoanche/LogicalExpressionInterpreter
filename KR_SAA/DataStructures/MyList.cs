using System;


namespace KR_SAA.DataStructures
{
    public class MyList<T>
    {
        private T[] _items; 
        private int _count; 

        public MyList(int capacity)
        {
            _items = new T[capacity];
            _count = 0;
        }
        public void Add(T item)
        {
            if (_count >= _items.Length)
            {
                throw new InvalidOperationException("List capacity exceeded.");
            }
            _items[_count++] = item;
        }
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }
                return _items[index];
            }
        }
        public int Count => _count;
       
    }
}
