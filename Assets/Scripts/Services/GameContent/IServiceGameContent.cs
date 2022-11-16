using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Commands.New;
using Solcery.Services.GameContent.Items;

namespace Solcery.Services.GameContent
{
    public interface IServiceGameContent
    {
        IItemTypes ItemTypes { get; }
        JArray Places { get; }
        JArray DragDrop { get; }
        JArray Tooltips { get; }
        HashSet<string> GameAttributes { get; }
        HashSet<string> CardAttributes { get; }
        List<Tuple<int, string>> Sounds { get; }

        int CommandIdForType(CommandTypesNew commandType);
        bool TryGetCommand(int commandId, out JObject command);
        bool TryGetCommand(CommandTypesNew commandType, out JObject command);
        void UpdateGameContent(JObject data);
        void UpdateGameContentOverrides(JObject data);
        void Cleanup();
    }
}