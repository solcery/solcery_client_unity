using System;
using TMPro;
using UnityEngine;

namespace Solcery.DebugViewers.States.Delays
{
    public sealed class DebugDelayStateLayout : MonoBehaviour
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