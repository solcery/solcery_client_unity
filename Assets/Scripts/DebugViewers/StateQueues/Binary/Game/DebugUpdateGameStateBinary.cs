using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary.Game.Attr;
using Solcery.DebugViewers.StateQueues.Binary.Game.Object;
using Solcery.Utils;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;
using UnityEngine.Pool;

namespace Solcery.DebugViewers.StateQueues.Binary.Game
{
    public sealed class DebugUpdateGameStateBinary : DebugUpdateStateBinary
    {
        private static readonly ObjectPool<DebugUpdateGameStateBinary> Pool;

        static DebugUpdateGameStateBinary()
        {
            Pool = new ObjectPool<DebugUpdateGameStateBinary>(Create, null, binary => binary.Cleanup());
        }

        public static DebugUpdateGameStateBinary Get()
        {
            return Pool.Get();
        }

        public static void Release(DebugUpdateGameStateBinary binary)
        {
            Pool.Release(binary);
        }

        public IReadOnlyList<IDUGSBAttrValue> Attrs => _attrList;
        public IReadOnlyList<int> RemovedObjectId => _removedObjectIdList;
        public IReadOnlyList<IDUGSBObjectValue> Objects => _objectList;

        private readonly List<IDUGSBAttrValue> _attrList;
        private readonly List<int> _removedObjectIdList;
        private readonly List<IDUGSBObjectValue> _objectList;

        private static DebugUpdateGameStateBinary Create()
        {
            return new DebugUpdateGameStateBinary();
        }

        private DebugUpdateGameStateBinary()
        {
            _attrList = new List<IDUGSBAttrValue>();
            _removedObjectIdList = new List<int>();
            _objectList = new List<IDUGSBObjectValue>();
        }

        protected override void FromJsonImpl(JObject value)
        {
            // attrs
            {
                var attrArray = value.GetValue<JArray>("attrs");
                foreach (var attrToken in attrArray)
                {
                    if (attrToken is JObject attrObject)
                    {
                        var attrValue = DUGSBAttrValue.Get();
                        attrValue.FromJson(attrObject);
                        _attrList.Add(attrValue);
                    }
                }
            }
            
            // removed object ids
            {
                if (value.TryGetValue("deleted_objects", out JArray rol))
                {
                    foreach (var rot in rol)
                    {
                        _removedObjectIdList.Add(rot.Value<int>());
                    }
                }
            }
            
            // objects
            {
                var objectArray = value.GetValue<JArray>("objects");
                foreach (var objectToken in objectArray)
                {
                    if (objectToken is JObject objectObject)
                    {
                        var objectValue = DUGSBObjectValue.Get();
                        objectValue.FromJson(objectObject);
                        _objectList.Add(objectValue);
                    }
                }
            }
        }

        protected override void FromBinaryImpl(IBinaryDataReader reader)
        {
            // read attrs
            {
                var count = reader.GetInt();
                while (_attrList.Count < count)
                {
                    var attrValue = DUGSBAttrValue.Get();
                    attrValue.FromBinary(reader);
                    _attrList.Add(attrValue);
                }
            }
            
            // read removed object ids
            {
                var count = reader.GetInt();
                while (_removedObjectIdList.Count < count)
                {
                    _removedObjectIdList.Add(reader.GetInt());
                }
            }

            // read objects
            {
                var count = reader.GetInt();
                while (_objectList.Count < count)
                {
                    var objectValue = DUGSBObjectValue.Get();
                    objectValue.FromBinary(reader);
                    _objectList.Add(objectValue);
                }
            }
        }

        protected override void ToBinaryImpl(IBinaryDataWriter writer)
        {
            // write attrs
            {
                writer.Put(_attrList.Count);
                foreach (var attrValue in _attrList)
                {
                    attrValue.ToBinary(writer);
                }
            }
            
            // write removed object ids
            {
                writer.Put(_removedObjectIdList.Count);
                foreach (var removedObjectId in _removedObjectIdList)
                {
                    writer.Put(removedObjectId);
                }
            }
            
            // write objects
            {
                writer.Put(_objectList.Count);
                foreach (var objectValue in _objectList)
                {
                    objectValue.ToBinary(writer);
                }
            }
        }

        protected override void CleanupImpl()
        {
            // cleanup attrs
            {
                while (_attrList.Count > 0)
                {
                    DUGSBAttrValue.Release(_attrList[0]);
                    _attrList.RemoveAt(0);
                }
            }
            
            // cleanup removed object ids
            _removedObjectIdList.Clear();
            
            // cleanup objects
            {
                while (_objectList.Count > 0)
                {
                    DUGSBObjectValue.Release(_objectList[0]);
                    _objectList.RemoveAt(0);
                }
            }
        }
    }
}