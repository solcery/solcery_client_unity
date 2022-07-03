using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues.Binary.Pause;
using UnityEngine;

namespace Solcery.DebugViewers.States.Pause
{
    public sealed class DebugPauseState : DebugState<DebugPauseStateLayout>
    {
        private readonly DebugStateViewPool<DebugPauseStateLayout> _viewPool;
        private readonly RectTransform _content;
        private readonly int _delayMSec;
        private readonly List<string> _fakeMoveToKeys;

        public static DebugPauseState Create(DebugUpdatePauseStateBinary binary, RectTransform content, DebugStateViewPool<DebugPauseStateLayout> viewPool)
        {
            return new DebugPauseState(binary, content, viewPool);
        }

        private DebugPauseState(DebugUpdatePauseStateBinary binary, RectTransform content, DebugStateViewPool<DebugPauseStateLayout> viewPool) : base(binary.Index)
        {
            _delayMSec = binary.DelayMSec;
            _content = content;
            _viewPool = viewPool;
            _fakeMoveToKeys = new List<string>();
        }
        
        public override void Draw(RectTransform content, JObject parameters)
        {
            Layout = _viewPool.Pop();
            Layout.transform.SetParent(_content);
            Layout.UpdatePosition(Vector3.zero);
            Layout.Delay.text = $"{_delayMSec}ms";
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