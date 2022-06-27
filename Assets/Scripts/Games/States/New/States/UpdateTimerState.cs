using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;

namespace Solcery.Games.States.New.States
{
    public sealed class UpdateTimerState : UpdateState
    {
        public bool IsStart { get; private set; }
        public int DurationMsec { get; private set; }
        public int TargetObjectId { get; private set; }
        
        public static UpdateState Create(ContextGameStateTypes updateStateType)
        {
            return new UpdateTimerState(updateStateType);
        }
        
        public UpdateTimerState(ContextGameStateTypes updateStateType) : base(updateStateType) { }

        public override void Init(JObject updateStateData)
        {
            IsStart = updateStateData.GetValue<bool>("start");
            DurationMsec = updateStateData.TryGetValue("duration", out int dms) ? dms : 0;
            TargetObjectId = updateStateData.TryGetValue("object_id", out int toi) ? toi : -1;
        }

        public override void Update(int deltaTimeMsec) { }

        public override bool CanRemove()
        {
            return true;
        }

        public override void Cleanup()
        {
            IsStart = false;
            DurationMsec = 0;
            TargetObjectId = -1;
        }
    }
}