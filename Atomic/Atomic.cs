namespace Atomizer
{
    public static class Atomic
    {
        public static AtomicValue<T> Struct<T>(T instance) where T : struct
            => new(instance);

        public static Atomic<T> Instance<T>(T instance) where T : class
            => new(instance);
    }
}

