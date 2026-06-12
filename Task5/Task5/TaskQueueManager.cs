using System.Collections.Concurrent;
using System.Diagnostics;

namespace Task5
{

    class TaskItem
    {
        private Guid Id;
        public string Description;
        public Action Action;
        private DateTime CreatedAt;

        public TaskItem(string Description, Action Action)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;

            this.Description = Description;
            this.Action = Action;
        }
    }

    internal class TaskQueueManager
    {

        private BlockingCollection<TaskItem> TaskQueue;

        public TaskQueueManager(int boundedCapacity)
        {
            TaskQueue = new BlockingCollection<TaskItem>(boundedCapacity);
        }

        public void AddTask(string description, Action taskAction)
        {
            TaskQueue.Add(new TaskItem(description, taskAction));
        }

        public void ProcessTasks(int workerCount)
        {

            CountdownEvent countdown = new CountdownEvent(workerCount);

            for (int i = 0; i < workerCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    while (!TaskQueue.IsCompleted)
                    {
                        TaskItem outParam;
                        if (TaskQueue.TryTake(out outParam))
                        {
                            outParam.Action.Invoke();
                        }
                    }
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();
        }

        public void CompleteAdding()
        {
            TaskQueue.CompleteAdding();
        }

        public int GetPendingTaskCount()
        {
            return TaskQueue.Count;
        }
    }
}
