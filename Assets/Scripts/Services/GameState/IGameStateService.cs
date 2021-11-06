namespace Solcery.Services.GameState
{
    public interface IGameStateService
    {
        public void Init();
        public void Update();
        public void Cleanup();
        public void Destroy();
    }
}