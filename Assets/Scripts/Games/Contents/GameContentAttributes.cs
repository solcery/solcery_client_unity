using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Games.Contents
{
    public sealed class GameContentAttributes : IGameContentAttributes
    {
        List<string> IGameContentAttributes.AttributeNameList => _attributeNameList;

        private readonly List<string> _attributeNameList;
        
        public static IGameContentAttributes Create()
        {
            return new GameContentAttributes();
        }

        private GameContentAttributes()
        {
            _attributeNameList = new List<string>();
        }

        void IGameContentAttributes.UpdateAttributesFromGameContent(JObject gameContent)
        {
            _attributeNameList.Clear();

            if (gameContent.TryGetValue(GameJsonKeys.GlobalCardAttributes, out JObject attrsObj)
                && attrsObj.TryGetValue("objects", out JArray attrsArray))
            {
                foreach (var attrToken in attrsArray)
                {
                    if (attrToken is JObject attrObj 
                        && attrObj.TryGetValue(GameJsonKeys.GlobalCardAttributeCode, out string name)
                        && !_attributeNameList.Contains(name))
                    {
                        _attributeNameList.Add(name);
                    }
                }
            }
        }
    }
}