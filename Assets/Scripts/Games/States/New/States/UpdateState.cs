using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;

namespace Solcery.Games.States.New.States
{
    public abstract class UpdateState
    {
        public int StateId { get; private set; }
        public bool IsPredictable { get; private set; }
        public ContextGameStateTypes UpdateStateType { get; }

        protected UpdateState(ContextGameStateTypes updateStateType)
        {
            IsPredictable = false;
            UpdateStateType = updateStateType;
        }

        public void Init(int id, JObject updateStateData, bool isPredictable)
        {
            IsPredictable = isPredictable;
            StateId = id;
            Init(updateStateData);
        }
        
        protected abstract void Init(JObject updateStateData);
        public abstract void Update(int deltaTimeMsec);
        public abstract bool CanRemove();
        public abstract void Cleanup();
    }
}