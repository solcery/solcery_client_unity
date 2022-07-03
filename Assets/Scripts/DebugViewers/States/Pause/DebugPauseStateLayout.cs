using TMPro;
using UnityEngine;

namespace Solcery.DebugViewers.States.Pause
{
    public sealed class DebugPauseStateLayout : MonoBehaviour
    {
        public TMP_Text Delay;

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