using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;
using UnityEngine.Pool;

namespace Solcery.DebugViewers.StateQueues.Binary.Pause
{
    public sealed class DebugUpdatePauseStateBinary : DebugUpdateStateBinary
    {
        private static readonly ObjectPool<DebugUpdatePauseStateBinary> Pool;

        static DebugUpdatePauseStateBinary()
        {
            Pool = new ObjectPool<DebugUpdatePauseStateBinary>(Create, null, binary => binary.Cleanup());
        }

        public static DebugUpdatePauseStateBinary Get()
        {
            return Pool.Get();
        }

        public static void Release(DebugUpdatePauseStateBinary binary)
        {
            Pool.Release(binary);
        }

        public int DelayMSec => _delayMSec;

        private int _delayMSec;

        private static DebugUpdatePauseStateBinary Create()
        {
            return new DebugUpdatePauseStateBinary();
        }
        
        private DebugUpdatePauseStateBinary() { }

        protected override void FromJsonImpl(JObject value)
        {
            if (value != null 
                && value.TryGetValue("delay", out int delay))
            {
                _delayMSec = delay;
            }
        }

        protected override void FromBinaryImpl(IBinaryDataReader reader)
        {
            _delayMSec = reader.PeekInt();
        }

        protected override void ToBinaryImpl(IBinaryDataWriter writer)
        {
            writer.Put(_delayMSec);
        }

        protected override void CleanupImpl()
        {
            _delayMSec = -1;
        }
    }
}