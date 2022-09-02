using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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

        void UpdateGameContent(JObject data);
        void UpdateGameContentOverrides(JObject data);
        void Cleanup();
    }
}