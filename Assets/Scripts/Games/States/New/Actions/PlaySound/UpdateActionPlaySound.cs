using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.PlaySound
{
    public sealed class UpdateActionPlaySound : UpdateAction
    {
        public int SoundId { get; }

        public static UpdateAction Create(JObject data)
        {
            return new UpdateActionPlaySound(data);
        }

        private UpdateActionPlaySound(JObject data) : base(data)
        {
            SoundId = data.GetValue<JObject>("value").GetValue<int>("sound_id");
        }
    }
}