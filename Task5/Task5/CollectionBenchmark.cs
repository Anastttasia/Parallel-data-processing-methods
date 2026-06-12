using System.Collections.Concurrent;
using System.Diagnostics;
using Task4;

namespace Task5
{
    internal class CollectionBenchmark
    {
        public ConcurrentLibraryCatalog concurrentLibraryCatalog;
        public LibraryCatalog libraryCatalog;
        public TaskQueueManager taskQueueManager;
        public ConcurrentCache concurrentCache;

        public Tuple<TimeSpan, int>? BenchmarkSynchronizedDictionary(int operationCount)
        {
            if (operationCount == 0)
            {
                return null;
            }

            int sucsessed = 0;
            int failured = 0;

            int maxAuthorLenght = 10;
            int maxTitleLenght = 15;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random rand = new Random(42);

            CountdownEvent countdown = new CountdownEvent(operationCount);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < operationCount; i++)
            {
                char[] author = new char[rand.Next(maxAuthorLenght)];
                char[] title = new char[rand.Next(maxTitleLenght)];

                Thread thread = new Thread(() =>
                {
                    int operationType = rand.Next(3);
                    if (operationType == 0)
                    {
                        libraryCatalog.AddBook(new string(title), new string(author));
                    }
                    if (operationType == 1)
                    {
                        libraryCatalog.SearchBooks(new string(title));
                    }

                    if (operationType == 2)
                    {
                        libraryCatalog.RemoveBook(new string(title));
                    }
                    countdown.Signal();
                });

                thread.Start();
            }

            countdown.Wait();
            stopwatch.Stop();

            return new Tuple<TimeSpan, int>(stopwatch.Elapsed, operationCount);
        }

        public Tuple<TimeSpan, int, int>? BenchmarkConcurrentDictionary(int operationCount)
        {
            if (operationCount == 0)
            {
                return null;
            }

            int sucsessed = 0;
            int failured = 0;

            int maxAuthorLenght = 10;
            int maxTitleLenght = 15;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random rand = new Random(42);

            CountdownEvent countdown = new CountdownEvent(operationCount);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < operationCount; i++)
            {
                char[] author = new char[rand.Next(maxAuthorLenght)];
                char[] title = new char[rand.Next(maxTitleLenght)];

                Thread thread = new Thread(() =>
                {
                    int operationType = rand.Next(3);
                    if (operationType == 0)
                    {
                        if (concurrentLibraryCatalog.AddBook(new string(title), new string(author)))
                        {
                            sucsessed++;
                        }
                        else
                        {
                            failured++;
                        }
                    }

                    if (operationType == 1)
                    {
                        concurrentLibraryCatalog.SearchBooks(new string(title));
                        sucsessed++;
                    }

                    if (operationType == 2)
                    {
                        if (concurrentLibraryCatalog.RemoveBook(new string(title)))
                        {
                            sucsessed++;
                        }
                        else
                        {
                            failured++;
                        }
                    }
                    countdown.Signal();
                });

                thread.Start();
            }

            countdown.Wait();
            stopwatch.Stop();

            return new Tuple<TimeSpan, int, int>(stopwatch.Elapsed, sucsessed, failured);
        }

        public Tuple<TimeSpan, int>? BenchmarkBlockingCollection(int taskCount, int workerCount)
        {
            if (workerCount == 0 || taskCount == 0)
            {
                return null;
            }

            int succesed = 0;

            for (int i = 0; i < taskCount; i++)
            {
                Action doSomething = new(() => succesed++);
                taskQueueManager.AddTask("do something", doSomething);
            }
            taskQueueManager.CompleteAdding();

            Stopwatch stopwatch = Stopwatch.StartNew();

            taskQueueManager.ProcessTasks(workerCount);

            stopwatch.Stop();

            return new Tuple<TimeSpan, int>(stopwatch.Elapsed, succesed);
        }

        public Tuple<TimeSpan, int, int>? BenchmarkConcurrentCache(int operationCount)
        {
            if (operationCount == 0)
            {
                return null;
            }

            int succesed = 0;
            int failed = 0;

            Random rand = new Random(42);

            Stopwatch stopwatch = Stopwatch.StartNew();

            CountdownEvent countdown = new CountdownEvent(operationCount);

            for (int i = 0; i < operationCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    int operationType = rand.Next(3);

                    if (operationType == 0)
                    {
                        if (concurrentCache.AddToCache("some_key", new object()))
                        {
                            succesed++;
                        }
                        else
                        {
                            failed++;
                        }
                    }

                    if (operationType == 1)
                    {
                        object someObject;

                        if (concurrentCache.TryGetFromCache("some_key", out someObject))
                        {
                            succesed++;
                        }
                        else
                        {
                            failed++;
                        }
                    }


                    if (operationType == 2)
                    {
                        if (concurrentCache.RemoveFromCache("some_key"))
                        {
                            succesed++;
                        }
                        else
                        {
                            failed++;
                        }
                    }

                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            stopwatch.Stop();

            return new Tuple<TimeSpan, int, int>(stopwatch.Elapsed, succesed, failed);
        }

        public void CompareAllCollections()
        {
            int operationCount = 1000;
            int taskCount = 1000;
            int workerCount = 10;
            int operationCountForCache = 500;

            Console.WriteLine("=== Результаты тестирования потокобезопасных коллекций ===");

            Tuple<TimeSpan, int>? resultSynchronizedDictionary = this.BenchmarkSynchronizedDictionary(operationCount);

            Console.WriteLine("Synchronized Dictionary");
            Console.WriteLine($"Количество операций: {operationCount}");
            Console.WriteLine($"Время выполнения: {resultSynchronizedDictionary.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Успешные операции: {resultSynchronizedDictionary.Item2}");
            Console.WriteLine($"Производительность: {resultSynchronizedDictionary.Item2 / resultSynchronizedDictionary.Item1.TotalSeconds} операций/сек");
            Console.WriteLine("");

            Tuple<TimeSpan, int, int>? resultConcurrentDictionary = this.BenchmarkConcurrentDictionary(operationCount);

            Console.WriteLine("ConcurrentDictionary");
            Console.WriteLine($"Количество операций: {operationCount}");
            Console.WriteLine($"Время выполнения: {resultConcurrentDictionary.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Успешные операции: {resultConcurrentDictionary.Item2}");
            Console.WriteLine($"Производительность: {resultConcurrentDictionary.Item2 / resultConcurrentDictionary.Item1.TotalSeconds} операций/сек");
            Console.WriteLine("");

            Tuple<TimeSpan, int>? resultBlockingCollection = this.BenchmarkBlockingCollection(taskCount, workerCount);

            Console.WriteLine("BlockingCollection");
            Console.WriteLine($"Количество задач: {taskCount}");
            Console.WriteLine($"Количество обработчиков: {workerCount}");
            Console.WriteLine($"Время выполнения: {resultBlockingCollection.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Обработанные задачи: {resultBlockingCollection.Item2}");
            Console.WriteLine($"Производительность: {resultBlockingCollection.Item2 / resultBlockingCollection.Item1.TotalSeconds} задач/сек");
            Console.WriteLine("");

            Tuple<TimeSpan, int, int>? resultConcurrentCache = this.BenchmarkConcurrentCache(operationCountForCache);

            Console.WriteLine("ConcurrentCache");
            Console.WriteLine($"Количество операций: {operationCountForCache}");
            Console.WriteLine($"Время выполнения: {resultConcurrentCache.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Успешные операции: {resultConcurrentCache.Item2}");
            Console.WriteLine("");

            Console.WriteLine("Сравнение производительности:");
            Console.WriteLine($"ConcurrentDictionary vs Synchronized Dictionary: {resultConcurrentDictionary.Item1.TotalMilliseconds / resultSynchronizedDictionary.Item1.TotalMilliseconds } x");
            Console.WriteLine("");
        }

    }
}
