using System;
using System.Collections.Generic;

namespace Solcery.Services.Resources.Loaders.Multi
{
    public sealed class MultiLoadTask : IMultiLoadTask
    {
        public event Action<bool, ILoadTask> Completed;

        private int _completedTaskCount;
        private List<ILoadTask> _tasks;

        public static IMultiLoadTask Create()
        {
            return new MultiLoadTask();
        }
        
        private MultiLoadTask()
        {
            _tasks = new List<ILoadTask>();
        }

        void IMultiLoadTask.AddTask(ILoadTask task)
        {
            task.Completed += OnTaskCompleted;
            _tasks.Add(task);
        }

        void ILoadTask.Run()
        {
            _completedTaskCount = 0;
            
            foreach (var task in _tasks)
            {
                task.Run();
            }
        }

        void ILoadTask.Destroy()
        {
            if (_tasks != null)
            {
                foreach (var task in _tasks)
                {
                    task.Destroy();
                }
            }

            _tasks?.Clear();
            _tasks = null;
        }
        
        private void OnTaskCompleted(bool obj, ILoadTask task)
        {
            ++_completedTaskCount;
            task.Completed -= OnTaskCompleted;
            task.Destroy();

            if (_tasks.Count <= _completedTaskCount)
            {
                Completed?.Invoke(true, this);
            }
        }
    }
}