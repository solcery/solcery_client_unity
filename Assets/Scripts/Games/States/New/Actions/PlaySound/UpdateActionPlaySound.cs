using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.PlaySound
{
    public sealed class UpdateActionPlaySound : UpdateAction
    {
        public int SoundId { get; }
        public int Volume { get; }

        public static UpdateAction Create(int stateId, JObject value)
        {
            return new UpdateActionPlaySound(stateId, value);
        }

        private UpdateActionPlaySound(int stateId, JObject value) : base(stateId)
        {
            SoundId = value.GetValue<int>("sound_id");
            Volume = value.GetValue<int>("volume");
        }
    }
}