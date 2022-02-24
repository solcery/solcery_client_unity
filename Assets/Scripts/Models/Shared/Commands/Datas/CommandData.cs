using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.Datas
{
    public abstract class CommandData
    {
        public static CommandTypes CommandTypeFromJson(JObject obj)
        {
            return obj.TryGetEnum("command_data_type", out CommandTypes result) 
                ? result 
                : CommandTypes.None;
        }
        
        public CommandTypes CommandType => GetCommandType();
        
        protected abstract CommandTypes GetCommandType();
        protected abstract void ConvertCommandToJson(JObject obj);
        
        public abstract void ApplyCommandToWorld(EcsWorld world);

        public JObject ToJson()
        {
            var result = new JObject {{"command_data_type", new JValue((int)CommandType)}};
            ConvertCommandToJson(result);
            return result;
        }
    }
}