using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary.Timer;
using UnityEngine;

namespace Solcery.DebugViewers.States.Timer
{
    public sealed class DebugTimerState : DebugState<DebugTimerStateLayout>
    {
        private readonly DebugStateViewPool<DebugTimerStateLayout> _viewPool;
        private readonly RectTransform _content;
        private readonly bool _isStart;
        private readonly int _durationMsec;
        private readonly int _targetObjectId;
        private readonly List<string> _fakeMoveToKeys;
        
        public static DebugTimerState Create(DebugUpdateTimerStateBinary binary, RectTransform content, DebugStateViewPool<DebugTimerStateLayout> viewPool)
        {
            return new DebugTimerState(binary, content, viewPool);
        }
        
        public DebugTimerState(DebugUpdateTimerStateBinary binary, RectTransform content, DebugStateViewPool<DebugTimerStateLayout> viewPool) : base(binary.Index)
        {
            _isStart = binary.IsStart;
            _durationMsec = binary.DurationMsec;
            _targetObjectId = binary.TargetObjectId;
            _content = content;
            _viewPool = viewPool;
            _fakeMoveToKeys = new List<string>();
        }

        public override void Draw(RectTransform content, JObject parameters)
        {
            Layout = _viewPool.Pop();
            Layout.transform.SetParent(_content);
            Layout.UpdatePosition(Vector3.zero);
            Layout.Timer.text = _isStart ? $"{_durationMsec}ms" : "Finished";
            Layout.TargetObjectId.text = _isStart ?  $"{_targetObjectId}" : "Finished";
        }

        public override void Cleanup()
        {
            _viewPool.Push(Layout);
            Layout = null;
        }

        public override IReadOnlyList<string> AllMoveToKeys()
        {
            return _fakeMoveToKeys;
        }

        public override Vector2 GetPositionToKeys(string key)
        {
            return new Vector2(0, 1);
        }
    }
}