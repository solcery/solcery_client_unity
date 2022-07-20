using Solcery.Types;
using TMPro;
using UnityEngine;

namespace Solcery.DebugViewers.Views.Attrs
{
    public sealed class DebugViewAttrLayout : MonoBehaviour
    {
        public Vector2 Size => _content.sizeDelta;

        public WorldRect WorldRect => WorldRect.Create(_content);

        [SerializeField]
        private TMP_Text keyValue;
        [SerializeField]
        private TMP_Text valueNewValue;
        [SerializeField]
        private TMP_Text valueOldValue;

        private RectTransform _content;

        private void Awake()
        {
            _content = gameObject.GetComponent<RectTransform>();
        }

        public void Apply(string key, string valueNew, string valueOld)
        {
            keyValue.text = key;
            valueNewValue.text = valueNew;
            valueOldValue.text = valueOld;
        }

        public void Cleanup()
        {
            keyValue.text = "";
            valueNewValue.text = "";
            valueOldValue.text = "";
        }

        public void UpdatePosition(Vector3 position)
        {
            _content.localPosition = position;
        }
        
        public void Enable(bool enable)
        {
            gameObject.SetActive(enable);
        }
    }
}