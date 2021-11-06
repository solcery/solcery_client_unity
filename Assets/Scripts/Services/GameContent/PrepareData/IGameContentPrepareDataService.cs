namespace Solcery.Services.GameContent.PrepareData
{
    public interface IGameContentPrepareDataService
    {
        public void Init();
        public void Cleanup();
        public void Destroy();
    }
}