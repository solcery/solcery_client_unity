using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Games.Contents
{
    public interface IGameContentAttributes
    {
        List<string> AttributeNameList { get; }

        void UpdateAttributesFromGameContent(JObject gameContent);
    }
}