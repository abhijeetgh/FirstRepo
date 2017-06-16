using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EZRAC.Core
{
    public class CustomSchedular : TaskScheduler, IDisposable
    {
        private BlockingCollection<Task> taskQueue;
        private Thread[] threads;

        public CustomSchedular(int concurrency)
        {
            taskQueue = new BlockingCollection<Task>();
            threads = new Thread[concurrency];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    foreach (Task t in taskQueue.GetConsumingEnumerable())
                    {
                        TryExecuteTask(t);
                    }
                }) { IsBackground = true };

                threads[i].Start();
            }
        }

        public int GetScheduledTaskCount { get { return GetScheduledTasks().Count(); } }

        protected override void QueueTask(Task task)
        {
            taskQueue.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // only allow inline execution if the executing thread is one
            // belonging to this scheduler
            if (threads.Contains(Thread.CurrentThread))
            {
                return TryExecuteTask(task);
            }
            else
            {
                return false;
            }
        }

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return threads.Length;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return taskQueue.ToArray();
        }

        public void Dispose()
        {
            taskQueue.CompleteAdding();
            // wait for each of the threads to finish
            foreach (Thread t in threads)
            {
                t.Join();
            }
        }
    }
}
