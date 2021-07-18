using System;

namespace Utils
{
    public class ReactiveProperty<T>
    {
        public T Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _value = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
        public event Action<T> ValueChanged;

        private T _value;

        public ReactiveProperty(T value)
        {
            _value = value;
        }
        
        public ReactiveProperty()
        {
            _value = default(T);
        }

        public void Set(T value)
        {
            Value = value;
        }
    }
}