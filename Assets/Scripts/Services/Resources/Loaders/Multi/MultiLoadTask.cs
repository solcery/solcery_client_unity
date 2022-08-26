using System;
using System.Collections.Generic;

namespace Solcery.Services.Resources.Loaders.Multi
{
    public sealed class MultiLoadTask : IMultiLoadTask
    {
        public event Action<bool, ILoadTask> Completed;
        public event Action<float> Progress;

        private int _completedTaskCount;
        private float _taskProgressWeight;
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
            task.Progress += OnTaskProgress;
            _tasks.Add(task);
        }

        void ILoadTask.Run()
        {
            _completedTaskCount = 0;
            _taskProgressWeight = 1f / _tasks.Count;
            
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
            task.Progress -= OnTaskProgress;
            task.Destroy();
            
            if (_tasks.Count <= _completedTaskCount)
            {
                Progress?.Invoke(1f);
                Completed?.Invoke(true, this);
            }
        }
        
        private void OnTaskProgress(float obj)
        {
            var progress = _taskProgressWeight * _completedTaskCount + obj / _tasks.Count;
            Progress?.Invoke(progress);
        }
    }
}