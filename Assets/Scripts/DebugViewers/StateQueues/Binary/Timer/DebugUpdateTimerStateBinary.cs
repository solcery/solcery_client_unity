using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;
using UnityEngine.Pool;

namespace Solcery.DebugViewers.StateQueues.Binary.Timer
{
    public sealed class DebugUpdateTimerStateBinary : DebugUpdateStateBinary
    {
        private static readonly ObjectPool<DebugUpdateTimerStateBinary> Pool;

        static DebugUpdateTimerStateBinary()
        {
            Pool = new ObjectPool<DebugUpdateTimerStateBinary>(Create, null, binary => binary.Cleanup());
        }

        public static DebugUpdateTimerStateBinary Get()
        {
            return Pool.Get();
        }

        public static void Release(DebugUpdateTimerStateBinary binary)
        {
            Pool.Release(binary);
        }
        
        public int Duration => _duration;
        
        private int _duration;

        private static DebugUpdateTimerStateBinary Create()
        {
            return new DebugUpdateTimerStateBinary();
        }
        
        private DebugUpdateTimerStateBinary() { }
        
        protected override void FromJsonImpl(JObject value)
        {
            throw new NotImplementedException();
        }

        protected override void FromBinaryImpl(IBinaryDataReader reader)
        {
            _duration = reader.PeekInt();
        }

        protected override void ToBinaryImpl(IBinaryDataWriter writer)
        {
            writer.Put(_duration);
        }

        protected override void CleanupImpl()
        {
            _duration = -1;
        }
    }
}