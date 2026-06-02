using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal class SynchronizationBenchmark
    {
        public Tuple<TimeSpan, decimal, bool>? BenchmarkNoSync(BankAccount account, List<decimal> transactions)
        {
            if (transactions == null || transactions.Count == 0 || account == null)
            {
                return null;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            decimal resultBalance = TransactionProcessor.ProcessTransactionsConcurrently(account, transactions);

            stopwatch.Stop();

            return new Tuple<TimeSpan, decimal, bool>(stopwatch.Elapsed, resultBalance, (resultBalance + account.GetBalance()) == 0);
        }

        public Tuple<TimeSpan, decimal, bool>? BenchmarkWithLock(BankAccount account, List<decimal> transactions)
        {
            if (transactions == null || transactions.Count == 0 || account == null)
            {
                return null;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            decimal resultBalance = TransactionProcessor.ProcessTransactionsWithLock(account, transactions);

            stopwatch.Stop();

            return new Tuple<TimeSpan, decimal, bool>(stopwatch.Elapsed, resultBalance, (resultBalance + account.GetBalance()) == 0);
        }

        public Tuple<TimeSpan, decimal, bool>? BenchmarkWithMonitor(BankAccount account, List<decimal> transactions)
        {
            if (transactions == null || transactions.Count == 0 || account == null)
            {
                return null;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            decimal resultBalance = TransactionProcessor.ProcessTransactionsWithMonitor(account, transactions);

            stopwatch.Stop();

            return new Tuple<TimeSpan, decimal, bool>(stopwatch.Elapsed, resultBalance, (resultBalance + account.GetBalance()) == 0);
        }

        public void CompareAllApproaches(List<decimal> transactions)
        {
            Console.WriteLine("");
            if (transactions == null || transactions.Count == 0)
            {
                Console.WriteLine("Ошибка входных параметров");
                return;
            }

            BankAccount account = new BankAccount();

            Tuple<TimeSpan, decimal, bool>? resultValue = null;

            double noSyncTotalTime = 1.0;
            double lockTotalTime = 1.0;
            double monitorTotalTime = 1.0;

            resultValue = BenchmarkNoSync(account, transactions);

            if (resultValue != null)
            {
                noSyncTotalTime = resultValue.Item1.TotalMilliseconds;
                Console.WriteLine("Без синхронизации:");
                Console.WriteLine($"\tВремя: {noSyncTotalTime} мс");
                Console.WriteLine($"\tИтоговый баланс: {resultValue.Item2}");
                Console.WriteLine($"\tКорректность: {(resultValue.Item3 ? "Да" : "Нет")}");
                Console.WriteLine($"\tГонки данных: {(resultValue.Item3 ? "Нет" : "Да")}");
                Console.WriteLine("");
            }

            account = new BankAccount();

            resultValue = BenchmarkWithLock(account, transactions);

            if (resultValue != null)
            {
                lockTotalTime = resultValue.Item1.TotalMilliseconds;
                Console.WriteLine("С использованием lock:");
                Console.WriteLine($"\tВремя: {lockTotalTime} мс");
                Console.WriteLine($"\tИтоговый баланс: {resultValue.Item2}");
                Console.WriteLine($"\tКорректность: {(resultValue.Item3 ? "Да" : "Нет")}");
                Console.WriteLine($"\tНакладные расходы: {((lockTotalTime / noSyncTotalTime) - 1.0):F3} %");
                Console.WriteLine("");
            }

            account = new BankAccount();

            resultValue = BenchmarkWithMonitor(account, transactions);

            if (resultValue != null)
            {
                monitorTotalTime = resultValue.Item1.TotalMilliseconds;
                Console.WriteLine("С использованием Monitor:");
                Console.WriteLine($"\tВремя: {monitorTotalTime} мс");
                Console.WriteLine($"\tИтоговый баланс: {resultValue.Item2}");
                Console.WriteLine($"\tКорректность: {(resultValue.Item3 ? "Да" : "Нет")}");
                Console.WriteLine($"\tНакладные расходы: {((monitorTotalTime / noSyncTotalTime) - 1.0):F3} %");
                Console.WriteLine("");
            }

            Console.WriteLine("Сравнение производительности:");
            Console.WriteLine($"\tУскорение lock vs Monitor: {(lockTotalTime / monitorTotalTime):F3}x");
            Console.WriteLine($"\tНакладные расходы lock: {((lockTotalTime / noSyncTotalTime) - 1.0):F3}%");
            Console.WriteLine($"\tНакладные расходы Monitor: {((monitorTotalTime / noSyncTotalTime) - 1.0):F3} %");
            Console.WriteLine("");
        }
    }
}
