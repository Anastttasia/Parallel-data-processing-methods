using System;
using System.Threading;


namespace Task2
{
    public delegate decimal[]? ProcessDataOperation(decimal[] data);

    internal class TaskProcessor
    {

        public decimal[]? ProcessDataWithThreadPool(decimal[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            
            int taskCount = 8;
            int arrayLenForThread = data.Length / taskCount;
            decimal[] resultArray = new decimal[data.Length];

            CountdownEvent countdown = new CountdownEvent(taskCount);

            for (int i = 0; i < taskCount; ++i)
            {
                decimal[] threadArray = new decimal[arrayLenForThread];

                int resultArrayIndex = i * arrayLenForThread;

                Array.Copy(data, resultArrayIndex, threadArray, 0, arrayLenForThread);

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    this.RealProcessData(ref threadArray);
                    Array.Copy(threadArray, 0, resultArray, resultArrayIndex, arrayLenForThread);
                    countdown.Signal();
                });
            }

            countdown.Wait();

            return resultArray;
        }

        public Task<decimal[]?> ProcessDataAsync(decimal[] data)
        {

            TaskCompletionSource<decimal[]?> tcs = new TaskCompletionSource<decimal[]?>();

            Task.Run(() =>
            {
                if (data == null || data.Length == 0)
                {
                    tcs.SetResult(null);
                }

                decimal[] result = new decimal[data.Length];
                Array.Copy(data, result, data.Length);
                this.RealProcessData(ref result);
                tcs.SetResult(result);
            });

            return tcs.Task;
        }

        public decimal[]? ProcessDataWithAPM(decimal[] data)
        {

            ProcessDataOperation operation = this.ProcessDataWithThreadPool;
            IAsyncResult asyncResult = operation.BeginInvoke(data, null, null);
            return operation.EndInvoke(asyncResult);
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


