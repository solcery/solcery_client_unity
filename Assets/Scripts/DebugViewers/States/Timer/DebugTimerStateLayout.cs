using TMPro;
using UnityEngine;

namespace Solcery.DebugViewers.States.Timer
{
    public sealed class DebugTimerStateLayout : MonoBehaviour
    {
        public TMP_Text Timer;
        public TMP_Text TargetObjectId;

        private RectTransform _content;

        private void Awake()
        {
            _content = GetComponent<RectTransform>();
        }


        public void UpdatePosition(Vector3 position)
        {
            _content.localPosition = position;
        }
    }
}