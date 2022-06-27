using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;

namespace Solcery.Games.States.New.States
{
    public abstract class UpdateState
    {
        public ContextGameStateTypes UpdateStateType { get; private set; }

        protected UpdateState(ContextGameStateTypes updateStateType)
        {
            UpdateStateType = updateStateType;
        }

        public abstract void Init(JObject updateStateData);
        public abstract void Update(int deltaTimeMsec);
        public abstract bool CanRemove();
        public abstract void Cleanup();
    }
}