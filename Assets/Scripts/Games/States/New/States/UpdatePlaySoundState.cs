using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Utils;

namespace Solcery.Games.States.New.States
{
    public sealed class UpdatePlaySoundState : UpdateState
    {
        public int SoundId { get; private set; }

        public static UpdateState Create(ContextGameStateTypes updateStateType)
        {
            return new UpdatePlaySoundState(updateStateType);
        }
        
        private UpdatePlaySoundState(ContextGameStateTypes updateStateType) : base(updateStateType) { }

        protected override void Init(JObject updateStateData)
        {
            SoundId = updateStateData.GetValue<int>("sound_id");
        }

        public override void Update(int deltaTimeMsec) { }

        public override bool CanRemove()
        {
            return true;
        }

        public override void Cleanup()
        {
            SoundId = -1;
        }
    }
}