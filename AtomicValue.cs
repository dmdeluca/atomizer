using System;
using System.Threading;

namespace Atomizer
{
    /// <summary>
    /// Provides methods for atomic operations on a struct (value type).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class AtomicValue<T> where T : struct
    {
        private T _object;
        private readonly object _lock;

        internal AtomicValue(T value)
        {
            _object = value;
            _lock = new();
        }

        /// <summary>
        /// Sets the value of the internal struct to the result of executing the given <paramref name="function"/> on the internal struct.
        /// </summary>
        /// <param name="function">The function to execute on the internal struct.</param>
        public void Set(Func<T, T> function)
        {
            lock (_lock)
            {
                _object = function(_object);
            }
        }

        /// <summary>
        /// Atomically reads the value of the struct and returns it
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T result;
            lock (_lock)
            {
                result = _object;
            }
            return result;
        }

        /// <summary>
        /// Gets the result of some <paramref name="function"/> executed on the current value of the internal struct.
        /// </summary>
        /// <typeparam name="K">The return type</typeparam>
        /// <param name="function">The function to execute on the struct</param>
        /// <returns>The result of the function</returns>
        public K Get<K>(Func<T, K> function)
        {
            K result;
            lock (_lock)
            {
                result = function(_object);
            }
            return result;
        }
    }
}

