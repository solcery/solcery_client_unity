using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Runtime.Contexts.Objects
{
    public interface IContextGameObjects
    {
        List<object> GetAllCardTypeObject();
        bool TryGetCardTypeData(object @object, out JObject cardTypeData);
        bool TryGetCardId(object @object, out int cardId);
        bool TryGetCardTypeId(object @object, out int cardTypeId);
    }
}