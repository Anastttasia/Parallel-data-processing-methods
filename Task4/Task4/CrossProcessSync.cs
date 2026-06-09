namespace Task4
{
    internal class CrossProcessSync
    {

        public void ExecuteWithGlobalLock(string mutexName, Action action)
        {
            using var mutex = new Mutex(false, mutexName);

            try
            {
                mutex.WaitOne();
                action.Invoke();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public bool TryExecuteWithGlobalLock(string mutexName, Action action, int timeoutMs)
        {
            using var mutex = new Mutex(false, mutexName);

            bool locked = mutex.WaitOne(timeoutMs);

            if (locked)
            {
                try
                {
                    action.Invoke();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            return locked;
        }
    }
}
