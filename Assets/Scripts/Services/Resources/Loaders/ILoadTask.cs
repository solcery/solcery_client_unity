using System;

namespace Solcery.Services.Resources.Loaders
{
    public interface ILoadTask
    {
        event Action<bool, ILoadTask> Completed;
        void Run();
        void Destroy();
    }
}