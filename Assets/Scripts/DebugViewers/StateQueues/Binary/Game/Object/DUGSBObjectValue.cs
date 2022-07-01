using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary.Game.Attr;
using Solcery.Utils;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;
using UnityEngine.Pool;

namespace Solcery.DebugViewers.StateQueues.Binary.Game.Object
{
    public sealed class DUGSBObjectValue : IDUGSBObjectValue
    {
        private static readonly ObjectPool<IDUGSBObjectValue> Pool;

        static DUGSBObjectValue()
        {
            Pool = new ObjectPool<IDUGSBObjectValue>(Create, null, binary => binary.Cleanup());
        }

        public static IDUGSBObjectValue Get()
        {
            return Pool.Get();
        }

        public static void Release(IDUGSBObjectValue binary)
        {
            Pool.Release(binary);
        }

        public bool IsNew => _isNew;
        public int Id => _id;
        public int TplId => _tplId;
        public IReadOnlyList<IDUGSBAttrValue> Attrs => _attrList;

        private bool _isNew;
        private int _id;
        private int _tplId;
        private readonly List<IDUGSBAttrValue> _attrList;

        private static IDUGSBObjectValue Create()
        {
            return new DUGSBObjectValue();
        }

        private DUGSBObjectValue()
        {
            _attrList = new List<IDUGSBAttrValue>();
        }

        public void FromJson(JObject objectValue)
        {
            _isNew = objectValue.TryGetValue("new", out bool isNew) && isNew;
            _id = objectValue.GetValue<int>("id");
            _tplId = objectValue.GetValue<int>("tplId");

            if (objectValue.TryGetValue("attrs", out JArray attrArray))
            {
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
        }

        public void FromBinary(IBinaryDataReader reader)
        {
            _id = reader.GetInt();
            _tplId = reader.GetInt();
            
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
        }

        public void ToBinary(IBinaryDataWriter writer)
        {
            writer.Put(_id);
            writer.Put(_tplId);
            
            // write attrs
            {
                writer.Put(_attrList.Count);
                foreach (var attrValue in _attrList)
                {
                    attrValue.ToBinary(writer);
                }
            }
        }

        public void Cleanup()
        {
            _id = -1;
            _tplId = -1;
            
            // cleanup attrs
            {
                while (_attrList.Count > 0)
                {
                    DUGSBAttrValue.Release(_attrList[0]);
                    _attrList.RemoveAt(0);
                }
            }
        }
    }
}