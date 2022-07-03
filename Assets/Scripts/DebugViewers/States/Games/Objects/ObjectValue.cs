using Solcery.DebugViewers.StateQueues.Binary.Game.Object;
using Solcery.DebugViewers.States.Games.Attrs;

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

        public static IObjectValue Create(IDUGSBObjectValue objectValue)
        {
            return new ObjectValue(objectValue);
        }
        
        private ObjectValue(IDUGSBObjectValue objectValue)
        {
            _isDestroyed = false;
            _isCreated = objectValue.IsNew;
            _id = objectValue.Id;
            _tplId = objectValue.TplId;
            _attrs = AttrsValue.Create(objectValue.Attrs);
        }
    }
}