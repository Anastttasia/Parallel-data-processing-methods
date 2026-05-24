using System.Diagnostics;


namespace Task1
{
    internal class PerformanceMeter
    {

        public static (TimeSpan, decimal[]?) MeasureExecutionTime(Func<decimal[]?> action, string operationName)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            decimal[]? resultOperation = action();
            stopwatch.Stop();

            Console.WriteLine($"{operationName}: {stopwatch.Elapsed.TotalMilliseconds} мс");

            return (stopwatch.Elapsed, resultOperation);
        }

        public static bool CompareResults(decimal[] result1, decimal[] result2, decimal tolerance = 0.0001m)
        {
            if (result1 == null || result2 == null) return false;

            if (result1.Length != result2.Length) return false;

            for (int i = 0; i < result1.Length; i++)
            {
                decimal diff = Math.Abs(result1[i] - result2[i]);

                if (diff > tolerance) return false;
            }

            return true;
        }

    }
}
