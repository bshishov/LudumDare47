using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Debugger
{
    [Serializable]
    public class CyclePool<T> : IEnumerable<T>, IEnumerable 
        where T : new()
    {
        private int _length;
        private int _cursor;
        private T[] _items;
        
        public CyclePool(int size)
        {
            if (size < 1) 
                throw new ArgumentOutOfRangeException(nameof(size));
            _items = new T[size];
        }

        public int Capacity => _items.Length;
        public int Length => _length;

        private int GetMoveCursor()
        {
            var c = _cursor;
            _cursor = (_cursor + 1) % _items.Length;
            return c;
        }

        public void Add(T item)
        {
            _items[GetMoveCursor()] = item;
            _length = Math.Min(_length + 1, _items.Length);
        }

        public T GetByIndexStartFromOldest(int index)
        {
            return _items[(index + _cursor) % _length];
        }
        
        public T GetByIndexStartFromNewest(int index)
        {
            return _items[(_cursor - 1 - index + _length) % _length];
        }

        public T GetOrCreate()
        {
            // This method helps to reduce allocations
            if (_length < _items.Length)
            {
                var item = new T();
                Add(item);
                return item;
            }
            
            return _items[GetMoveCursor()];
        }
        
        public void Clear()
        {
            _items = new T[_items.Length];
        }
        
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        private struct Enumerator : IEnumerator<T>
        {
            private int _index;
            private readonly CyclePool<T> _collection;

            internal Enumerator(CyclePool<T> collection)
            {
                _collection = collection;
                _index = -1;
                Current = default;
            }

            public bool MoveNext()
            {
                if (++_index >= _collection.Length)
                    return false;
                
                Current = _collection.GetByIndexStartFromOldest(_index);
                return true;
            }

            public void Reset()
            {
                _index = -1;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
    }
}