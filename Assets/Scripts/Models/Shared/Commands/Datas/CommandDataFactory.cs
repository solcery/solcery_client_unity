using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Commands.Datas.OnClick;

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
            }

            return null;
        }
    }
}