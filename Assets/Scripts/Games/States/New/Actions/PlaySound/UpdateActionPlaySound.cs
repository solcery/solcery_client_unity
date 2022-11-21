using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.PlaySound
{
    public sealed class UpdateActionPlaySound : UpdateAction
    {
        public int SoundId { get; }
        public int Volume { get; }

        public static UpdateAction Create(JObject data)
        {
            return new UpdateActionPlaySound(data);
        }

        private UpdateActionPlaySound(JObject data) : base(data)
        {
            var value = data.GetValue<JObject>("value");
            SoundId = value.GetValue<int>("sound_id");
            Volume = value.GetValue<int>("volume");
        }
    }
}