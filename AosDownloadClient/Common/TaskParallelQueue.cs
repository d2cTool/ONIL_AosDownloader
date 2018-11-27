using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AosP2PClient.Common
{
    public class TaskParallelQueue<T1, T2> where T1 : EventArgs
    {
        private readonly ConcurrentQueue<Task<T1>> processingQueue = new ConcurrentQueue<Task<T1>>();
        private readonly ConcurrentDictionary<int, Task> runningTasks = new ConcurrentDictionary<int, Task>();
        private readonly int maxParallelizationCount;
        private readonly int maxQueueLength;
        private TaskCompletionSource<bool> tscQueue = new TaskCompletionSource<bool>();

        public event EventHandler<T1> DownloadPartExecuted;

        public TaskParallelQueue(int maxParallelizationCount = 1, int maxQueueLength = 30)
        {
            this.maxParallelizationCount = maxParallelizationCount;
            this.maxQueueLength = maxQueueLength;
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

        private Task<T1> CreateTask(Func<T2, T1> func, T2 arg)
        {
            Task<T1> task = new Task<T1>(() => func(arg));
            task.ContinueWith(ErrorHandling, TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(SuccesHandling, TaskContinuationOptions.OnlyOnRanToCompletion);
            return task;
        }

        private void SuccesHandling(Task<T1> task)
        {
            DownloadPartExecuted?.Invoke(this, task.Result);
            StartTasks();
        }

        private void ErrorHandling(Task<T1> task)
        {
            //logger.Error(task.Exception, "@DownloadingQueue");
            //TaskExecuted?.Invoke(this, new TaskExecutedEventArgs());
            StartTasks();
        }

        public int GetQueueCount()
        {
            return processingQueue.Count;
        }

        public int GetRunningCount()
        {
            return runningTasks.Count;
        }

        public void StartTasks()
        {
            var startMaxCount = maxParallelizationCount - runningTasks.Count;
            for (int i = 0; i < startMaxCount; i++)
            {
                if (!processingQueue.TryDequeue(out Task<T1> futureTask))
                {
                    break;
                }

                futureTask.Start();
                if (!runningTasks.TryAdd(futureTask.GetHashCode(), futureTask))
                {
                    throw new Exception("Should not happen, hash codec are unique");
                }

                futureTask.ContinueWith((t2) =>
                {
                    if (!runningTasks.TryRemove(t2.GetHashCode(), out Task temp))
                    {
                        throw new Exception("Should not happen, hash codes are unique");
                    }
                    StartTasks();
                });

                if (processingQueue.IsEmpty && runningTasks.IsEmpty)
                {
                    var oldQueue = Interlocked.Exchange(ref tscQueue, new TaskCompletionSource<bool>());
                    oldQueue.TrySetResult(true);
                }
            }
        }
    }
}
