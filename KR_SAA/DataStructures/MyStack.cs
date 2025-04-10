using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KR_SAA.DataStructures
{
    public class MyStack<T>
    {
        private T[] _items;
        private int _count;

        public MyStack(int capacity)
        {
            _items = new T[capacity];
            _count = 0;
        }

        public void Push(T item)
        {
            if (_count >= _items.Length)
            {
                throw new InvalidOperationException("Stack overflow: Cannot push more items.");
            }
            _items[_count++] = item;
        }

        public T Pop()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Stack underflow: Cannot pop from an empty stack.");
            }
            return _items[--_count];
        }

        public T Peek()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Stack underflow: Cannot peek an empty stack.");
            }
            return _items[_count - 1];
        }

        public int Count => _count;
    }
}
