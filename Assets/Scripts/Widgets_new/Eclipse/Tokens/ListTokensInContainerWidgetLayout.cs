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
    }
}
