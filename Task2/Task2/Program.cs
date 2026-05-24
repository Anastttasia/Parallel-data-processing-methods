using System.Threading.Tasks;
using Task1;
using Task2;

int seed = 42;
int arraySize = 10_000_000;
int threadsCount = 4;

decimal[] originalData = new decimal[arraySize];

Random rand = new Random(seed);
Task1.DataProcessor processorOld = new Task1.DataProcessor();
Task2.TaskProcessor processorNew = new Task2.TaskProcessor();


for (int i = 0; i < originalData.Length; i++)
{
    originalData[i] = (decimal)(1.0 + rand.NextDouble() * 999.0);
}

decimal[]? ProcessSequential()
{
    return processorOld.ProcessDataSequential(originalData);
}

decimal[]? ProcessParallel()
{
    return processorOld.ProcessDataParallel(originalData, threadsCount);
}

decimal[]? ProcessThreadPool()
{
    return processorNew.ProcessDataWithThreadPool(originalData);
}

decimal[]? ProcessAsync()
{
    Task<decimal[]?> task = processorNew.ProcessDataAsync(originalData);
    task.Wait();
    return task.Result;
}

decimal[]? ProcessWithAPM()
{
    return processorNew.ProcessDataWithAPM(originalData);
}

(TimeSpan seqTime, decimal[]? seqRes) = PerformanceMeter.MeasureExecutionTime(ProcessSequential, "Последовательная обработка");
(TimeSpan parallelTime, decimal[]? parallelRes) = PerformanceMeter.MeasureExecutionTime(ProcessParallel, "Параллельная обработка");
(TimeSpan threadPoolTime, decimal[]? threadPoolRes) = PerformanceMeter.MeasureExecutionTime(ProcessThreadPool, "ThreadPool обработка");
(TimeSpan asyncTime, decimal[]? asyncRes) = PerformanceMeter.MeasureExecutionTime(ProcessAsync, "TAP обработка");
//(TimeSpan apmTime, decimal[]? apmRes) = PerformanceMeter.MeasureExecutionTime(ProcessWithAPM, "APM обработка");

bool isEqualsParallel = Task1.PerformanceMeter.CompareResults(seqRes, parallelRes);
bool isEqualsThreadPool = Task1.PerformanceMeter.CompareResults(seqRes, threadPoolRes);
bool isEqualsAsync = Task1.PerformanceMeter.CompareResults(seqRes, asyncRes);
//bool isEqualsApm = Task1.PerformanceMeter.CompareResults(seqRes, apmRes);

Console.WriteLine("=== Результаты обработки ===");
Console.WriteLine($"{4 / 2}");
Console.WriteLine($"Размер данных: {arraySize} элементов");
Console.WriteLine("");
Console.WriteLine($"Последовательная обработка: {seqTime.TotalMilliseconds} мс");
Console.WriteLine("");
Console.WriteLine($"Параллельная обработка ({threadsCount} потока): {parallelTime.TotalMilliseconds} мс");
Console.WriteLine($"Ускорение: {seqTime.TotalMilliseconds / parallelTime.TotalMilliseconds} x");
Console.WriteLine($"Результаты совпадают: {(isEqualsParallel ? "Да" : "Нет")}");
Console.WriteLine("");
Console.WriteLine($"ThreadPool обработка: {threadPoolTime.TotalMilliseconds} мс");
Console.WriteLine($"Ускорение: {seqTime.TotalMilliseconds / threadPoolTime.TotalMilliseconds} x");
Console.WriteLine($"Результаты совпадают: {(isEqualsThreadPool ? "Да" : "Нет")}");
Console.WriteLine("");
Console.WriteLine($"ThreadPool обработка: {asyncTime.TotalMilliseconds} мс");
Console.WriteLine($"Ускорение: {seqTime.TotalMilliseconds / asyncTime.TotalMilliseconds} x");
Console.WriteLine($"Результаты совпадают: {(isEqualsAsync ? "Да" : "Нет")}");
//Console.WriteLine("");
//Console.WriteLine($"ThreadPool обработка: {apmTime.TotalMilliseconds} мс");
//Console.WriteLine($"Ускорение: {seqTime.TotalMilliseconds / apmTime.TotalMilliseconds} x");
//Console.WriteLine($"Результаты совпадают: {(isEqualsApm ? "Да" : "Нет")}");
Console.WriteLine("");