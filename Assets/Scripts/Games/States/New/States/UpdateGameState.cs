using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;

namespace Solcery.Games.States.New.States
{
    public sealed class UpdateGameState : UpdateState
    {
        public JObject GameState { get; private set; }

        public static UpdateState Create(ContextGameStateTypes updateStateType)
        {
            return new UpdateGameState(updateStateType);
        }

        private UpdateGameState(ContextGameStateTypes updateStateType) : base(updateStateType) { }
        
        protected override void Init(JObject updateStateData)
        {
            GameState = updateStateData;
        }

        public override void Update(int deltaTimeMsec) { }

        public override bool CanRemove()
        {
            return true;
        }

        public override void Cleanup()
        {
            GameState = null;
        }
    }
}