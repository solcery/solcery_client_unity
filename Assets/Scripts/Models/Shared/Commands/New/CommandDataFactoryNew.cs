using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Commands.New.OnClick;

namespace Solcery.Models.Shared.Commands.New
{
    public static class CommandDataFactoryNew
    {
        public static CommandDataNew CreateFromJson(JObject obj)
        {
            var commandType = CommandDataNew.CommandTypeFromJson(obj);

            switch (commandType)
            {
                case CommandTypesNew.OnLeftClick:
                    return CommandOnLeftClickDataNew.CreateFromJson(obj);
            }

            return null;
        }
    }
}