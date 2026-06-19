namespace Task6
{
    internal class LockCounter
    {

        private long realValue = 0;
        private Lock _lockObj = new();

        public long Value
        {
            get
            {
                return realValue;
            }
        }

        public LockCounter(long initialValue)
        {
            lock (_lockObj)
            {
                realValue = initialValue;
            }
        }

        public void Increment()
        {
            lock (_lockObj)
            {
                realValue += 1;
            }
        }

        public void Decrement()
        {
            lock (_lockObj)
            {
                realValue -= 1;
            }
        }

        public void Add(long value)
        {
            lock (_lockObj)
            {
                realValue += value;
            }
        }

        public void Exchange(long newValue)
        {
            lock (_lockObj)
            {
                realValue = newValue;
            }
        }

        public void CompareExchange(long newValue, long comparand)
        {
            lock (_lockObj)
            {
                if (realValue == comparand)
                {
                    realValue = newValue;
                }
            }
        }

        public void Reset()
        {
            lock (_lockObj)
            {
                realValue = 0;
            }
        }

        public long GetAndIncrement()
        {
            lock (_lockObj)
            {
                realValue += 1;
                return realValue;
            }
        }

        public long GetAndDecrement()
        {
            lock (_lockObj)
            {
                realValue -= 1;
                return realValue;
            }
        }

    }
}
