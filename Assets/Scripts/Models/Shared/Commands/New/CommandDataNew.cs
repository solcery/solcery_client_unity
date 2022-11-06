using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.New
{
    public abstract class CommandDataNew
    {
        public int CommandId => _commandId;
        public CommandTypesNew CommandType => _commandType;

        private static int _id = 0;

        private readonly int _cid;
        private readonly int _commandId;
        private readonly CommandTypesNew _commandType;

        public static void ResetId()
        {
            _id = 0;
        }
        
        public static CommandTypesNew CommandTypeFromJson(JObject obj)
        {
            return obj.TryGetEnum("command_data_type", out CommandTypesNew result) 
                ? result 
                : CommandTypesNew.None;
        }

        protected CommandDataNew(int commandId, CommandTypesNew commandType)
        {
            _id++;
            _cid = _id;
            _commandId = commandId;
            _commandType = commandType;
        }
        
        protected abstract void ConvertCommandToJson(JObject obj);
        
        public abstract void ApplyCommandToWorld(EcsWorld world);

        public JObject ToJson()
        {
            var result = new JObject
            {
                {"cid", new JValue(_cid)},
                {"command_data_type", new JValue((int)_commandType)},
                {"command_id", new JValue(_commandId)}
            };
            ConvertCommandToJson(result);
            return result;
        }
    }
}