namespace Task3
{
    internal class TransactionProcessor
    {
        public static decimal ProcessTransactionsConcurrently(BankAccount account, List<decimal> transactions)
        {
            if (transactions == null || transactions.Count == 0 || account == null)
            {
                return 0.0M;
            }

            CountdownEvent countdown = new CountdownEvent(transactions.Count);

            BankAccount accountResult = new BankAccount();

            for (int i = 0; i < transactions.Count; i++)
            {
                decimal transaction = transactions[i];
                Thread thread = new Thread(() =>
                {
                    account.Transfer(accountResult, transaction);
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            return accountResult.GetBalance();
        }

        public static decimal ProcessTransactionsWithLock(BankAccount account, List<decimal> transactions)
        {
            if (transactions == null || transactions.Count == 0 || account == null)
            {
                return 0.0M;
            }

            CountdownEvent countdown = new CountdownEvent(transactions.Count);

            BankAccount accountResult = new BankAccount();

            for (int i = 0; i < transactions.Count; i++)
            {
                decimal transaction = transactions[i];
                Thread thread = new Thread(() =>
                {
                    account.TransferWithLock(accountResult, transaction);
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            return accountResult.GetBalance();
        }

        public static decimal ProcessTransactionsWithMonitor(BankAccount account, List<decimal> transactions)
        {
            if (transactions == null || transactions.Count == 0 || account == null)
            {
                return 0.0M;
            }

            CountdownEvent countdown = new CountdownEvent(transactions.Count);

            BankAccount accountResult = new BankAccount();

            for (int i = 0; i < transactions.Count; i++)
            {
                decimal transaction = transactions[i];
                Thread thread = new Thread(() =>
                {
                    account.TransferWithMonitor(accountResult, transaction);
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            return accountResult.GetBalance();
        }

        public static bool ProcessConcurrentTransfers(List<BankAccount> accounts, int transferCount)
        {
            if (accounts == null || accounts.Count == 0 || transferCount == 0)
            {
                return false;
            }

            Random rand = new Random();

            CountdownEvent countdownDeadLock = new CountdownEvent(2);

            BankAccount client1 = accounts[rand.Next() % accounts.Count];
            BankAccount client2 = accounts[rand.Next() % accounts.Count];
            decimal transferValue = (decimal)(10.0 + rand.NextDouble() * 990.0) * (rand.NextDouble() > 0.5 ? 1.0M : -1.0M);

            while (client1 == client2)
            {
                client2 = accounts[rand.Next() % accounts.Count];
            }

            Thread thread1 = new Thread(() =>
            {
                bool isSucsess = client1.WithdrawWithTimeout(transferValue, 15);

                if (isSucsess)
                {
                    isSucsess &= client2.DepositWithTimeout(transferValue, 15);
                }

                if (isSucsess)
                {
                    countdownDeadLock.Signal();
                }
            });
            

            Thread thread2 = new Thread(() =>
            {
                bool isSucsess = client2.WithdrawWithTimeout(transferValue, 15);

                if (isSucsess)
                {
                    isSucsess &= client1.DepositWithTimeout(transferValue, 15);
                }

                if (isSucsess)
                {
                    countdownDeadLock.Signal();
                }
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            bool hadDeadLock = countdownDeadLock.CurrentCount == 0;

            CountdownEvent countdown = new CountdownEvent(transferCount);

            for (int i = 0; i < transferCount; i++)
            {
                BankAccount sender = accounts[rand.Next() % accounts.Count];
                BankAccount reciever = accounts[rand.Next() % accounts.Count];
                transferValue = (decimal)(10.0 + rand.NextDouble() * 990.0);

                while (sender == reciever)
                {
                    reciever = accounts[rand.Next() % accounts.Count];
                }

                Thread thread = new Thread(() =>
                {
                    bool isSucsessWithdraw = false;
                    bool isSucsessDeposit = false;

                    while (!isSucsessWithdraw)
                    {
                        isSucsessWithdraw &= sender.WithdrawWithTimeout(transferValue, 15);
                    }

                    while (!isSucsessDeposit)
                    {
                        isSucsessDeposit &= reciever.DepositWithTimeout(transferValue, 15);
                    }

                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            return hadDeadLock;
        }
    }
}
