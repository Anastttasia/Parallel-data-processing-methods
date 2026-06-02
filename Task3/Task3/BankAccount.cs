namespace Task3
{
    internal class BankAccount
    {
        private decimal Balance = 0;
        private Lock _lockObj = new Lock();
        private object _monitorObj = new ();

        public decimal GetBalance()
        {
            return Balance;
        }

        //Basic no SYNC
        public void Deposit(decimal amount)
        {
            this.Balance = this.Balance + amount;
        }

        public void Withdraw(decimal amount)
        {
            this.Balance = this.Balance - amount;
        }

        public void Transfer(BankAccount target, decimal amount)
        {
            this.Withdraw(amount);
            target.Deposit(amount);
        }

        //LOCK SYNC
        public void DepositWithLock(decimal amount)
        {
            _lockObj.Enter();
            try
            {
                this.Deposit(amount);
            }
            finally { _lockObj.Exit(); }
        }

        public void WithdrawWithLock(decimal amount)
        {
            _lockObj.Enter();
            try
            {
                this.Withdraw(amount);
            }
            finally { _lockObj.Exit(); }
        }

        public void TransferWithLock(BankAccount target, decimal amount)
        {
            _lockObj.Enter();
            try
            {
                this.Withdraw(amount);
                target.DepositWithLock(amount);
            }
            finally { _lockObj.Exit(); }
        }

        //MONITOR SYNC
        public void DepositWithMonitor(decimal amount)
        {
            bool lockTaken = false;
            try
            {
                Monitor.Enter(_monitorObj, ref lockTaken);
                this.Deposit(amount);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_monitorObj);
            }
        }

        public void WithdrawWithMonitor(decimal amount)
        {
            bool lockTaken = false;
            try
            {
                Monitor.Enter(_monitorObj, ref lockTaken);
                this.Withdraw(amount);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_monitorObj);
            }
        }

        public void TransferWithMonitor(BankAccount target, decimal amount)
        {
            bool lockTaken = false;
            try
            {
                Monitor.Enter(_monitorObj, ref lockTaken);

                this.Withdraw(amount);
                target.DepositWithMonitor(amount);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_monitorObj);
            }
        }

        //TIMEOUTS SYNC
        public bool DepositWithTimeout(decimal amount, int timeoutMs)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_monitorObj, timeoutMs, ref lockTaken);
                if (lockTaken)
                {
                    this.Deposit(amount);
                }
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_monitorObj);
            }
            return lockTaken;
        }

        public bool WithdrawWithTimeout(decimal amount, int timeoutMs)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_monitorObj, timeoutMs, ref lockTaken);
                if (lockTaken)
                {
                    this.Withdraw(amount);
                }
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_monitorObj);
            }
            return lockTaken;
        }
    }
}
