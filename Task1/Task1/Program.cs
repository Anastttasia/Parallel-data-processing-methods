using Task1;

int seed = 42;
int arraySize = 10_000_000;
int threadsCount = 4;

decimal[] originalData = new decimal[arraySize];

Random rand = new Random(seed);
Task1.DataProcessor processor = new Task1.DataProcessor();


for (int i = 0; i < originalData.Length; i++)
{
    originalData[i] = (decimal) (1.0 + rand.NextDouble() * 999.0);
}

decimal[]? ProcessSequential()
{
    return processor.ProcessDataSequential(originalData);
}

decimal[]? ProcessParallel()
{
    return processor.ProcessDataParallel(originalData, threadsCount);
}

(TimeSpan seqTime, decimal[]? seqRes) = PerformanceMeter.MeasureExecutionTime(ProcessSequential, "Последовательная обработка");
(TimeSpan parallelTime, decimal[]? parallelRes) = PerformanceMeter.MeasureExecutionTime(ProcessParallel, "Параллельная обработка");

bool isEquals = Task1.PerformanceMeter.CompareResults(seqRes, parallelRes);

Console.WriteLine("=== Результаты обработки ===");
Console.WriteLine($"Размер данных: {arraySize} элементов");
Console.WriteLine($"Последовательная обработка: {seqTime.TotalMilliseconds} мс");
Console.WriteLine($"Параллельная обработка ({threadsCount} потока): {parallelTime.TotalMilliseconds} мс");
Console.WriteLine($"Ускорение: {seqTime.TotalMilliseconds / parallelTime.TotalMilliseconds} x");
Console.WriteLine($"Результаты совпадают: {(isEquals ? "Да" : "Нет")}");
