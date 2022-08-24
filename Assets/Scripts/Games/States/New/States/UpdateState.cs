using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;

namespace Solcery.Games.States.New.States
{
    public abstract class UpdateState
    {
        public bool IsPredictable { get; private set; }
        public ContextGameStateTypes UpdateStateType { get; private set; }

        protected UpdateState(ContextGameStateTypes updateStateType)
        {
            IsPredictable = false;
            UpdateStateType = updateStateType;
        }

        public void Init(JObject updateStateData, bool isPredictable)
        {
            IsPredictable = isPredictable;
            Init(updateStateData);
        }
        
        protected abstract void Init(JObject updateStateData);
        public abstract void Update(int deltaTimeMsec);
        public abstract bool CanRemove();
        public abstract void Cleanup();
    }
}