using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Commands.Datas.OnClick;
using Solcery.Models.Shared.Commands.Datas.OnDrop;

namespace Solcery.Models.Shared.Commands.Datas
{
    public static class CommandDataFactory
    {
        public static CommandData CreateFromJson(JObject obj)
        {
            var commandType = CommandData.CommandTypeFromJson(obj);

            switch (commandType)
            {
                case CommandTypes.OnLeftClick:
                    return CommandOnLeftClickData.CreateFromJson(obj);
                
                case CommandTypes.OnRightClick:
                    return CommandOnRightClickData.CreateFromJson(obj);
                
                case CommandTypes.OnDrop:
                    return CommandOnDropData.CreateFromJson(obj);
            }

            return null;
        }
    }
}