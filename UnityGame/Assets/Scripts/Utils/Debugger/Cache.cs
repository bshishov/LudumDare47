#if DEBUG
#define USE_REFLECTION
#endif

using System;
using System.Collections.Generic;

#if USE_REFLECTION
#endif

namespace Utils.Debugger
{
    public class Cache<TKey, TVal>
    {
        private readonly TVal _default;
        private readonly Func<TKey, TVal> _defaultFn;
        private readonly Dictionary<TKey, TVal> _storage = new Dictionary<TKey, TVal>();
        private readonly bool _useDefault;
        private readonly bool _useDefaultFn;

        public Cache()
        {
        }

        public Cache(TVal defaultVal) : this()
        {
            _useDefault = true;
            _default = defaultVal;
        }

        public Cache(Func<TKey, TVal> defaultFn) : this()
        {
            _useDefaultFn = defaultFn != null;
            _defaultFn = defaultFn;
        }

        public TVal Get(TKey key)
        {
            TVal val;
            if (_storage.TryGetValue(key, out val))
                return val;

            if (_useDefaultFn)
            {
                var newVal = _defaultFn(key);
                _storage[key] = newVal;
                return newVal;
            }

            if (_useDefault)
                return _default;

            throw new KeyNotFoundException($"Missing {key.ToString()}");
        }

        public TVal Get(TKey key, Func<TVal> createFn)
        {
            TVal val;
            if (_storage.TryGetValue(key, out val))
                return val;

            if (createFn != null)
            {
                var newVal = createFn();
                _storage[key] = newVal;
                return newVal;
            }

            if (_useDefault)
                return _default;

            throw new KeyNotFoundException($"Missing {key.ToString()}");
        }

        public bool ContainsKey(TKey key)
        {
            return _storage.ContainsKey(key);
        }

        public void Set(TKey key, TVal val)
        {
            if (_storage.ContainsKey(key))
                _storage[key] = val;
            else
                _storage.Add(key, val);
        }

        public void Clear()
        {
            _storage.Clear();
        }
    }
}