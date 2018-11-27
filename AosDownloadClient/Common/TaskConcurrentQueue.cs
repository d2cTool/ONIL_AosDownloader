using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AosP2PClient.Common
{
    public class TaskConcurrentQueue<T1, T2> where T1 : EventArgs
    {
        private readonly ConcurrentQueue<Task<T1>> processingQueue = new ConcurrentQueue<Task<T1>>();
        private readonly int maxQueueLength;
        private bool hasRunning;

        public event EventHandler<T1> TaskExecuted;

        public TaskConcurrentQueue(int maxQueueLength = 100)
        {
            this.maxQueueLength = maxQueueLength;
            hasRunning = false;
        }

        public void StartTasks()
        {
            if (processingQueue.IsEmpty || hasRunning)
            {
                return;
            }

            if (processingQueue.TryDequeue(out Task<T1> task))
            {
                task.Start();
                hasRunning = true;
            }
        }

        private Task<T1> CreateTask(Func<T2, T1> func, T2 arg)
        {
            Task<T1> task = new Task<T1>(() => func(arg));
            task.ContinueWith(ErrorHandling, TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(SuccesHandling, TaskContinuationOptions.OnlyOnRanToCompletion);
            return task;
        }

        private void ErrorHandling(Task<T1> task)
        {
            //logger.Error(task.Exception);
            //TaskExecuted?.Invoke(this, new TaskExecutedEventArgs());
            hasRunning = false;
            StartTasks();
        }

        private void SuccesHandling(Task<T1> task)
        {
            TaskExecuted?.Invoke(this, task.Result);
            hasRunning = false;
            StartTasks();
        }

        public bool Queue(Func<T2, T1> func, T2 arg)
        {
            if (processingQueue.Count < maxQueueLength)
            {
                Task<T1> task = CreateTask(func, arg);
                processingQueue.Enqueue(task);
                return true;
            }
            return false;
        }

        public int GetQueueCount()
        {
            return processingQueue.Count;
        }
    }
}
