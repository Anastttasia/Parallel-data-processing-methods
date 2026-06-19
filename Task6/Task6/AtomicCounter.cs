namespace Task6
{
    internal class AtomicCounter
    {
        private long realValue = 0;

        public long Value
        {
            get
            {
                return realValue;
            }
        }

        public AtomicCounter(long initialValue)
        {
            Interlocked.Exchange(ref realValue, initialValue);
        }

        public void Increment()
        {
            Interlocked.Increment(ref realValue);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref realValue);
        }

        public void Add(long value)
        {
            Interlocked.Add(ref realValue, value);
        }

        public void Exchange(long newValue)
        {
            Interlocked.Exchange(ref realValue, newValue);
        }

        public void CompareExchange(long newValue, long comparand)
        {
            Interlocked.CompareExchange(ref realValue, newValue, comparand);
        }

        public void Reset()
        {
            Interlocked.Exchange(ref realValue, 0);
        }

        public long GetAndIncrement()
        {
            return Interlocked.Increment(ref realValue);
        }

        public long GetAndDecrement()
        {
            return Interlocked.Decrement(ref realValue);
        }

    }
}
