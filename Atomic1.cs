using System;
using System.Threading;

namespace Atomizer
{
    /// <summary>
    /// Provides atomic operations on a <typeparamref name="T"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the instance</typeparam>
    public class Atomic<T> where T : class
    {
        protected T _object;
        protected readonly object _lock;

        internal Atomic(T value)
        {
            _object = value;
            _lock = new();
        }

        /// <summary>
        /// Atomically performs the <paramref name="action"/> on the instance.
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action<T> action)
        {
            lock (_lock)
            {
                action(_object);
            }
        }

        /// <summary>
        /// Atomically retrieves the value of some member of the instance. If the member itself is not a value and not an atomic type, the result is not guaranteed to be thread-safe.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public K Get<K>(Func<T, K> function)
        {
            lock (_lock)
            {
                var result = function(_object);
                return result;
            }
        }
    }
}

