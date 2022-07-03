using System.Collections.Generic;
using Solcery.DebugViewers.StateQueues.Binary.Game.Object;

namespace Solcery.DebugViewers.States.Games.Objects
{
    public sealed class ObjectsValue : IObjectsValue
    {
        IReadOnlyList<string> IObjectsValue.ObjectKeys => _objectKeys;
        IReadOnlyList<IObjectValue> IObjectsValue.Objects => _objects;

        private readonly List<string> _objectKeys;
        private readonly List<IObjectValue> _objects;

        public static IObjectsValue Create(IReadOnlyList<IDUGSBObjectValue> objects)
        {
            return new ObjectsValue(objects);
        }

        private ObjectsValue(IReadOnlyList<IDUGSBObjectValue> objects)
        {
            _objects = new List<IObjectValue>();
            _objectKeys = new List<string>();
            foreach (var objectValue in objects)
            {
                _objectKeys.Add(objectValue.Id.ToString());
                _objects.Add(ObjectValue.Create(objectValue));
            }
        }
    }
}