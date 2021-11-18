namespace Solcery.Services.Resources.Loaders.Multi
{
    public interface IMultiLoadTask : ILoadTask
    {
        void AddTask(ILoadTask task);
    }
}