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
        List<Tuple<int, string>> Sounds { get; }
        Dictionary<CommandTypesNew, JObject> Commands { get; }

        int CommandIdForType(CommandTypesNew commandType);
        void UpdateGameContent(JObject data);
        void UpdateGameContentOverrides(JObject data);
        void Cleanup();
    }
}