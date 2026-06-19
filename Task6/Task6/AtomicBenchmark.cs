using System.Diagnostics;

namespace Task6
{
    internal class AtomicBenchmark
    {
        public Tuple<TimeSpan, long, bool>? BenchmarkInterlockedCounter(int operationCount, int threadCount)
        {
            if (operationCount == 0 || threadCount == 0)
            {
                return null;
            }

            int operationCountPerThread = operationCount / threadCount;

            AtomicCounter counterBenchmark = new AtomicCounter(0L);

            CountdownEvent countdown = new CountdownEvent(operationCount);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    for (int j = 0; j < operationCountPerThread; j++)
                    {
                        counterBenchmark.Increment();
                        countdown.Signal();
                    }
                });

                thread.Start();
            }

            countdown.Wait();
            stopwatch.Stop();

            return new Tuple<TimeSpan, long, bool>(stopwatch.Elapsed, counterBenchmark.Value, counterBenchmark.Value == operationCount);
        }

        public Tuple<TimeSpan, long, bool>? BenchmarkLockedCounter(int operationCount, int threadCount)
        {
            if (operationCount == 0 || threadCount == 0)
            {
                return null;
            }

            int operationCountPerThread = operationCount / threadCount;

            LockCounter counterBenchmark = new LockCounter(0L);

            CountdownEvent countdown = new CountdownEvent(operationCount);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    for (int j = 0; j < operationCountPerThread; j++)
                    {
                        counterBenchmark.Increment();
                        countdown.Signal();
                    }
                });

                thread.Start();
            }

            countdown.Wait();
            stopwatch.Stop();

            return new Tuple<TimeSpan, long, bool>(stopwatch.Elapsed, counterBenchmark.Value, counterBenchmark.Value == operationCount);
        }

        public Tuple<TimeSpan, long, bool>? BenchmarkLockFreeStack(int operationCount, int threadCount)
        {
            if (operationCount == 0 || threadCount == 0)
            {
                return null;
            }

            LockFreeStack<int> stack = new LockFreeStack<int>();
            StatisticsTracker tracker = new StatisticsTracker();

            for (int i = 0; i < operationCount; i++)
            {
                stack.Push(i);
            }

            CountdownEvent countdown = new CountdownEvent(operationCount);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < operationCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    int res = 0;
                    tracker.RecordRequest(stack.TryPop(out res), 1);
                    countdown.Signal();
                });

                thread.Start();
            }

            countdown.Wait();
            stopwatch.Stop();

            return new Tuple<TimeSpan, long, bool>(stopwatch.Elapsed, tracker.GetSuccessCount(), stack.IsEmpty());
        }

        public void CompareAllApproaches()
        {
            int operationCount = 100000;
            int threadCount = 100;

            Console.WriteLine("=== Сравнение атомарных операций и блокировок ===");

            Tuple<TimeSpan, long, bool>? resultInterlockedCounter = this.BenchmarkInterlockedCounter(operationCount, threadCount);

            Console.WriteLine("Interlocked Counter");
            Console.WriteLine($"Количество операций: {operationCount}");
            Console.WriteLine($"Количество потоков: {threadCount}");
            Console.WriteLine($"Время выполнения: {resultInterlockedCounter.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Итоговое значение: {resultInterlockedCounter.Item2}");
            Console.WriteLine($"Корректность: {(resultInterlockedCounter.Item3 ? "Да" : "Нет")}");
            Console.WriteLine("");

            Tuple<TimeSpan, long, bool>? resultLockedCounter = this.BenchmarkLockedCounter(operationCount, threadCount);

            Console.WriteLine("Locked Counter");
            Console.WriteLine($"Количество операций: {operationCount}");
            Console.WriteLine($"Количество потоков: {threadCount}");
            Console.WriteLine($"Время выполнения: {resultLockedCounter.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Итоговое значение: {resultLockedCounter.Item2}");
            Console.WriteLine($"Корректность: {(resultLockedCounter.Item3 ? "Да" : "Нет")}");
            Console.WriteLine("");

            Tuple<TimeSpan, long, bool>? resultLockFreeStack = this.BenchmarkLockFreeStack(operationCount, threadCount);

            Console.WriteLine("Lock-Free Stack");
            Console.WriteLine($"Количество операций: {operationCount}");
            Console.WriteLine($"Количество потоков: {threadCount}");
            Console.WriteLine($"Время выполнения: {resultLockFreeStack.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Успешные операции: {resultLockFreeStack.Item2}");
            Console.WriteLine($"Корректность: {(resultLockFreeStack.Item3 ? "Да" : "Нет")}"); Console.WriteLine("");

            Console.WriteLine("Сравнение производительности");
            Console.WriteLine($"Interlocked vs Locked: {resultInterlockedCounter.Item1.TotalMilliseconds / resultLockedCounter.Item1.TotalMilliseconds}");
            Console.WriteLine($"Накладные расходы блокировок: {(resultLockedCounter.Item1.TotalMilliseconds / resultInterlockedCounter.Item1.TotalMilliseconds - 1.0) * 100}%");
        }
    }
}
