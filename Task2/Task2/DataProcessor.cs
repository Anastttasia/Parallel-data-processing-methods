using System;
using System.Threading;

namespace Task1
{
    internal class DataProcessor
    {

        public decimal[]? ProcessDataSequential(decimal[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            decimal[] result = new decimal[data.Length];

            Array.Copy(data, result, data.Length);

            this.RealProcessData(ref result);

            return result;
        }

        public decimal[]? ProcessDataParallel(decimal[] data, int threadCount)
        {
            if (data == null || data.Length == 0 || threadCount <= 0)
            {
                return null;
            }

            int arrayLenForThread = data.Length / threadCount;
            decimal[] resultArray = new decimal[data.Length];

            CountdownEvent countdown = new CountdownEvent(threadCount);

            for (int i = 0; i < threadCount; ++i)
            {
                decimal[] threadArray = new decimal[arrayLenForThread];

                int resultArrayIndex = i * arrayLenForThread;

                Array.Copy(data, resultArrayIndex, threadArray, 0, arrayLenForThread);

                Thread thread = new Thread(() =>
                {
                    this.RealProcessData(ref threadArray);
                    Array.Copy(threadArray, 0, resultArray, resultArrayIndex, arrayLenForThread);
                    countdown.Signal();
                });

                thread.Start();
            }

            countdown.Wait();

            return resultArray;
        }

        private void RealProcessData(ref decimal[] result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (decimal)Math.Sqrt((double)result[i]) * (decimal)Math.Log10((double)result[i] + 1);
            }
        }
    }


}
