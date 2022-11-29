using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contexts.GameStates.Actions.Sound
{
    public sealed class ContextGameStateActionPlaySound : ContextGameStateAction
    {
        private readonly int _soundId;
        private readonly int _volume;

        public static ContextGameStateAction Create(int soundId, int volume)
        {
            return new ContextGameStateActionPlaySound(soundId, volume);
        }

        private ContextGameStateActionPlaySound(int soundId, int volume)
        {
            _soundId = soundId;
            _volume = volume;
        }

        public override JObject ToJson(int id, int stateId)
        {
            var result = new JObject
            {
                { "id", new JValue(id) },
                { "state_id", new JValue(stateId) },
                { "action_type", new JValue((int)ContextGameStateActionTypes.PlaySound) }
            };

            var value = new JObject
            {
                { "sound_id", new JValue(_soundId) },
                { "volume", new JValue(_volume) }
            };
            result.Add("value", value);

            return result;
        }
    }
}