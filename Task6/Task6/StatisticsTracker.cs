namespace Task6
{
    internal class StatisticsTracker
    {
        private AtomicCounter TotalRequests = new AtomicCounter(0L);
        private AtomicCounter SuccessfulRequests = new AtomicCounter(0L);
        private AtomicCounter FailedRequests = new AtomicCounter(0L);
        private AtomicCounter TotalProcessingTime = new AtomicCounter(0L);

        public void RecordRequest(bool success, long processingTime)
        {
            if (success)
            {
                SuccessfulRequests.Increment();
            }
            else
            {
                FailedRequests.Increment();
            }

            TotalRequests.Increment();

            TotalProcessingTime.Add(processingTime);
        }

        public long GetSuccessRate()
        {
            return SuccessfulRequests.Value / TotalRequests.Value;
        }

        public long GetAverageProcessingTime()
        {
            return TotalProcessingTime.Value / TotalRequests.Value;
        }

        public long GetSuccessCount()
        {
            return SuccessfulRequests.Value;
        }

        public void Reset()
        {
            TotalRequests.Reset();
            TotalProcessingTime.Reset();
            SuccessfulRequests.Reset();
            FailedRequests.Reset();
        }
    }
}
