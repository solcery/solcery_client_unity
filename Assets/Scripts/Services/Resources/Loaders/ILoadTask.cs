using System;

namespace Solcery.Services.Resources.Loaders
{
    public interface ILoadTask
    {
        event Action<bool, ILoadTask> Completed;
        event Action<float> Progress;
        void Run();
        void Destroy();
    }
}