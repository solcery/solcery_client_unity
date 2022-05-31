using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.States.Games.Attrs;
using Solcery.Utils;

namespace Solcery.DebugViewers.States.Games.Objects
{
    public sealed class ObjectValue : IObjectValue
    {
        bool IObjectValue.IsDestroyed => _isDestroyed;
        bool IObjectValue.IsCreated => _isCreated;
        int IObjectValue.Id => _id;
        int IObjectValue.TplId => _tplId;
        IAttrsValue IObjectValue.Attrs => _attrs;

        private readonly bool _isDestroyed;
        private readonly bool _isCreated;
        private readonly int _id;
        private readonly int _tplId;
        private readonly IAttrsValue _attrs;

        public static IObjectValue Create(JObject currentObject, JObject oldJObject)
        {
            return new ObjectValue(currentObject, oldJObject);
        }
        
        private ObjectValue(JObject currentObject, JObject oldJObject)
        {
            _isDestroyed = currentObject == null;
            _isCreated = oldJObject == null;

            if (currentObject != null)
            {
                _id = currentObject.GetValue<int>("id");
                _tplId = currentObject.GetValue<int>("tplId");
            }
            else
            {
                if (oldJObject != null)
                {
                    _id = oldJObject.GetValue<int>("id");
                    _tplId = oldJObject.GetValue<int>("tplId");
                }
            }

            _attrs = AttrsValue.Create(currentObject?.GetValue<JArray>("attrs"), oldJObject?.GetValue<JArray>("attrs"));
        }
    }
}