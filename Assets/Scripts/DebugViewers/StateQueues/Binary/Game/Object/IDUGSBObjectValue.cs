using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary.Game.Attr;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;

namespace Solcery.DebugViewers.StateQueues.Binary.Game.Object
{
    public interface IDUGSBObjectValue
    {
        bool IsNew { get; }
        int Id { get; }
        int TplId { get; }
        IReadOnlyList<IDUGSBAttrValue> Attrs { get; }
        void FromJson(JObject objectValue);
        void FromBinary(IBinaryDataReader reader);
        void ToBinary(IBinaryDataWriter writer);
        void Cleanup();
    }
}