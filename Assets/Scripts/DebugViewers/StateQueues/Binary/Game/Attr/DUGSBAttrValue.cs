using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;
using UnityEngine.Pool;

namespace Solcery.DebugViewers.StateQueues.Binary.Game.Attr
{
    public sealed class DUGSBAttrValue : IDUGSBAttrValue
    {
        private static readonly ObjectPool<IDUGSBAttrValue> Pool;

        static DUGSBAttrValue()
        {
            Pool = new ObjectPool<IDUGSBAttrValue>(Create, null, binary => binary.Cleanup());
        }

        public static IDUGSBAttrValue Get()
        {
            return Pool.Get();
        }

        public static void Release(IDUGSBAttrValue binary)
        {
            Pool.Release(binary);
        }
        
        string IDUGSBAttrValue.Key => _key;
        int IDUGSBAttrValue.Current => _current;
        int IDUGSBAttrValue.Preview => _preview;

        private string _key;
        private int _current;
        private int _preview;

        private static IDUGSBAttrValue Create()
        {
            return new DUGSBAttrValue();
        }
        
        private DUGSBAttrValue() { }

        void IDUGSBAttrValue.FromJson(JObject objectValue)
        {
            _key = objectValue.GetValue<string>("key");
            _current = objectValue.GetValue<int>("current");
            _preview = objectValue.GetValue<int>("preview");
        }

        void IDUGSBAttrValue.FromBinary(IBinaryDataReader reader)
        {
            _key = reader.GetString();
            _current = reader.GetInt();
            _preview = reader.GetInt();
        }

        void IDUGSBAttrValue.ToBinary(IBinaryDataWriter writer)
        {
            writer.Put(_key);
            writer.Put(_current);
            writer.Put(_preview);
        }

        void IDUGSBAttrValue.Cleanup()
        {
            _key = string.Empty;
            _current = -1;
            _preview = -1;
        }
    }
}