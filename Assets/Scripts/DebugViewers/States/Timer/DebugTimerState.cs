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
        private readonly int _timerMSec;
        private readonly List<string> _fakeMoveToKeys;
        
        public static DebugTimerState Create(DebugUpdateTimerStateBinary binary, RectTransform content, DebugStateViewPool<DebugTimerStateLayout> viewPool)
        {
            return new DebugTimerState(binary, content, viewPool);
        }
        
        public DebugTimerState(DebugUpdateTimerStateBinary binary, RectTransform content, DebugStateViewPool<DebugTimerStateLayout> viewPool) : base(binary.Index)
        {
            _timerMSec = binary.Duration;
            _content = content;
            _viewPool = viewPool;
            _fakeMoveToKeys = new List<string>();
        }

        public override void Draw(RectTransform content, JObject parameters)
        {
            Layout = _viewPool.Pop();
            Layout.transform.SetParent(_content);
            Layout.UpdatePosition(Vector3.zero);
            Layout.Timer.text = $"{_timerMSec}ms";
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