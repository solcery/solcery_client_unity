using Newtonsoft.Json.Linq;
using Solcery.Utils;
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

        public bool IsStart => _isStart;
        public int DurationMsec => _durationMsec;
        public int TargetObjectId => _targetObjectId;

        private bool _isStart;
        private int _durationMsec;
        private int _targetObjectId;

        private static DebugUpdateTimerStateBinary Create()
        {
            return new DebugUpdateTimerStateBinary();
        }
        
        private DebugUpdateTimerStateBinary() : base(DebugStateTypes.Timer) { }
        
        protected override void FromJsonImpl(JObject value)
        {
            if (value != null)
            {
                _isStart = value.GetValue<bool>("start");
                _durationMsec = value.TryGetValue("duration", out int dms) ? dms : 0;
                _targetObjectId = value.TryGetValue("object_id", out int toi) ? toi : -1;
            }
        }

        protected override void FromBinaryImpl(IBinaryDataReader reader)
        {
            _isStart = reader.GetBool();

            if (!_isStart)
            {
                return;
            }
            
            _durationMsec = reader.GetInt();
            _targetObjectId = reader.GetInt();
        }

        protected override void ToBinaryImpl(IBinaryDataWriter writer)
        {
            writer.Put(_isStart);
            
            if (!_isStart)
            {
                return;
            }
            
            writer.Put(_durationMsec);
            writer.Put(_targetObjectId);
        }

        protected override void CleanupImpl()
        {
            _isStart = false;
            _durationMsec = 0;
            _targetObjectId = -1;
        }
    }
}