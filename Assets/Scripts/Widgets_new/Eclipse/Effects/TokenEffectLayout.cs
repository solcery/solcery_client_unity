using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Effects
{
    public class TokenEffectLayout : MonoBehaviour
    {
        public Image Image;
        public RectTransform RectTransform;

        [SerializeField]
        private GameObject moveAnimation;
        [SerializeField]
        private GameObject destroyAnimation;

        public void UpdateMoveAnimation(bool isShow)
        {
            moveAnimation.SetActive(isShow);
        }

        public void UpdateDestroyAnimation(bool isShow)
        {
            destroyAnimation.SetActive(isShow);
        }
    }
}
