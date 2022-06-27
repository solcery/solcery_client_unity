using Solcery.Games.States;

namespace Solcery.DebugViewers
{
    public interface IDebugViewer
    {
        void Show();
        void Hide();
        //void AddGameStatePackage(GameStatePackage gameStatePackage);
    }
}