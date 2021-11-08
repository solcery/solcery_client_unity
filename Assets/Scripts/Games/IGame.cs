using Solcery.BrickInterpretation;
using Solcery.Models;
using Solcery.Services.GameContent;
using Solcery.Services.GameContent.PrepareData;
using Solcery.Services.GameState;
using Solcery.Services.Transport;

namespace Solcery.Games
{
    public interface IGame
    {
        public ITransportService TransportService { get; }
        public IBrickService BrickService { get; }
        public IGameContentService GameContentService { get; }
        public IGameContentPrepareDataService GameContentPrepareDataService { get; }
        public IGameStateService GameStateService { get; }
        public IModel Model { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}