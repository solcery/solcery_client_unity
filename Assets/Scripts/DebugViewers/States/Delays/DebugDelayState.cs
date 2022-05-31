using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.DebugViewers.States.Delays
{
    public sealed class DebugDelayState : DebugState<DebugDelayStateLayout>
    {
        private DebugStateViewPool<DebugDelayStateLayout> _viewPool;
        private RectTransform _content;
        private readonly int _delayMSec;
        private readonly List<string> _fakeMoveToKeys;

        public static DebugDelayState Create(int stateIndex, int delayMSec, RectTransform content, DebugStateViewPool<DebugDelayStateLayout> viewPool)
        {
            return new DebugDelayState(stateIndex, delayMSec, content, viewPool);
        }

        private DebugDelayState(int stateIndex, int delayMSec, RectTransform content, DebugStateViewPool<DebugDelayStateLayout> viewPool) : base(stateIndex)
        {
            _delayMSec = delayMSec;
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