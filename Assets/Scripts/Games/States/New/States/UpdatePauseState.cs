using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;

namespace Solcery.Games.States.New.States
{
    public sealed class UpdatePauseState : UpdateState
    {
        private int _delay;
        
        public static UpdateState Create(ContextGameStateTypes updateStateType)
        {
            return new UpdatePauseState(updateStateType);
        }
        
        public UpdatePauseState(ContextGameStateTypes updateStateType) : base(updateStateType) { }

        public override void Init(JObject updateStateData)
        {
            _delay = updateStateData.GetValue<int>("delay");
        }

        public override void Update(int deltaTimeMsec)
        {
            _delay -= deltaTimeMsec;
        }

        public override bool CanRemove()
        {
            return _delay <= 0;
        }

        public override void Cleanup()
        {
            _delay = 0;
        }
    }
}