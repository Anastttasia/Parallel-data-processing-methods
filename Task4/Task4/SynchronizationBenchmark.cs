using System.Diagnostics;

namespace Task4
{
    internal class SynchronizationBenchmark
    {
        public Tuple<TimeSpan, TimeSpan, TimeSpan>? BenchmarkReaderWriterLock(LibraryCatalog catalog, int readerCount, int writerCount)
        {
            if (readerCount == 0 || writerCount == 0)
            {
                return null;
            }

            int maxAuthorLenght = 10;
            int maxTitleLenght = 15;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random rand = new Random(42);

            CountdownEvent countdownReader = new CountdownEvent(readerCount);
            CountdownEvent countdownWriter = new CountdownEvent(writerCount);

            TimeSpan writerTime, readerTime;

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < writerCount; i++)
            {
                char[] author = new char[rand.Next(maxAuthorLenght)];
                char[] title = new char[rand.Next(maxTitleLenght)];

                Thread thread = new Thread(() =>
                {
                    catalog.AddBook(new string(title), new string(author));
                    countdownWriter.Signal();
                });
                thread.Start();
            }

            countdownWriter.Wait();
            stopwatch.Stop();

            writerTime = stopwatch.Elapsed;

            stopwatch.Restart();
            for (int i = 0; i < readerCount; i++)
            {
                char[] author = new char[rand.Next(maxAuthorLenght)];
                char[] title = new char[rand.Next(maxTitleLenght)];

                Thread thread = new Thread(() =>
                {
                    catalog.GetAllBooks();
                    countdownReader.Signal();
                });
                thread.Start();
            }
            
            countdownReader.Wait();

            readerTime = stopwatch.Elapsed;

            return new Tuple<TimeSpan, TimeSpan, TimeSpan>(readerTime, writerTime, readerTime + writerTime);
        }

        public Tuple<TimeSpan, int, int>? BenchmarkSemaphore(ResourcePool pool, int requestCount)
        {
            if (requestCount == 0)
            {
                return null;
            }

            int succesed = 0, failed = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();

            CountdownEvent countdown = new CountdownEvent(requestCount);

            for (int i = 0; i < requestCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    if (pool.TryAcquireResource(10))
                    {
                        succesed++;
                    }
                    else
                    {
                        failed++;
                    }
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            stopwatch.Stop();

            return new Tuple<TimeSpan, int, int>(stopwatch.Elapsed, succesed, failed);
        }

        public Tuple<TimeSpan, int>? BenchmarkMutex(CrossProcessSync syncer, int operationCount)
        {
            if (operationCount == 0)
            {
                return null;
            }

            int succesed = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();

            CountdownEvent countdown = new CountdownEvent(operationCount);

            for (int i = 0; i < operationCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    syncer.TryExecuteWithGlobalLock("MyMutex", () => succesed++, 10);
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();

            stopwatch.Stop();

            return new Tuple<TimeSpan, int>(stopwatch.Elapsed, succesed);
        }

        public void CompareAllPrimitives(LibraryCatalog library, ResourcePool pool, CrossProcessSync syncer)
        {
            int readingCount = 50;
            int writingCount = 10;
            int requestCount = 100;
            int operationCount = 20;

            Console.WriteLine("=== Результаты тестирования примитивов синхронизации ===");

            Tuple<TimeSpan, TimeSpan, TimeSpan>? resultLocker = this.BenchmarkReaderWriterLock(library, readingCount, writingCount);

            Console.WriteLine("ReaderWriterLockSlim");
            Console.WriteLine($"Количество операций чтения: {readingCount}");
            Console.WriteLine($"Количество операций записи: {writingCount}");
            Console.WriteLine($"Время чтения: {resultLocker.Item1.TotalMilliseconds} мс");
            Console.WriteLine($"Время записи: {resultLocker.Item2.TotalMilliseconds} мс");
            Console.WriteLine($"Общее время: {resultLocker.Item3.TotalMilliseconds} мс");
            Console.WriteLine("");

            Tuple<TimeSpan, int, int>? resultPool = this.BenchmarkSemaphore(pool, requestCount);

            Console.WriteLine("SemaphoreSlim");
            Console.WriteLine($"Количество запросов: {requestCount}");
            Console.WriteLine($"Доступных ресурсов: {pool.AvailableCount}");
            Console.WriteLine($"Успешные запросы: {resultPool.Item2}");
            Console.WriteLine($"Неудачные запросы: {resultPool.Item3}");
            Console.WriteLine($"Время выполнения: {resultPool.Item1.TotalMilliseconds} мс");
            Console.WriteLine("");

            Tuple<TimeSpan, int>? resultMutex = this.BenchmarkMutex(syncer, operationCount);

            Console.WriteLine("Mutex");
            Console.WriteLine($"Количество операций: {operationCount}");
            Console.WriteLine($"Успешные запросы: {resultMutex.Item2}");
            Console.WriteLine($"Время выполнения: {resultMutex.Item1.TotalMilliseconds} мс");
            Console.WriteLine("");
        }

    }
}
