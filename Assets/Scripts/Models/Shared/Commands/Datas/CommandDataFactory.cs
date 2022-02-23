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
                case CommandTypes.OnClick:
                    return CommandOnClickData.CreateFromJson(obj);
                
                case CommandTypes.OnDrop:
                    return CommandOnDropData.CreateFromJson(obj);
            }

            return null;
        }
    }
}