using Newtonsoft.Json.Linq;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;

namespace Solcery.DebugViewers.StateQueues.Binary.Game.Attr
{
    public interface IDUGSBAttrValue
    {
        string Key { get; }
        int Current { get; }
        int Preview { get; }
        void FromJson(JObject objectValue);
        void FromBinary(IBinaryDataReader reader);
        void ToBinary(IBinaryDataWriter writer);
        void Cleanup();
    }
}