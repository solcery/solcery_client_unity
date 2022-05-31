using TMPro;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Tokens
{
    public class ListTokensInContainerWidgetLayout : MonoBehaviour
    {
        public RectTransform Content;
        public TextMeshProUGUI Counter;

        public void UpdateParent(Transform parent)
        {
            transform.SetParent(parent, false);
        }

        public void UpdateCounter(int count)
        {
            var active = count > 0;
            Content.gameObject.SetActive(active);
            Counter.gameObject.SetActive(active);
            Counter.text = count.ToString();
        }
    }
}
