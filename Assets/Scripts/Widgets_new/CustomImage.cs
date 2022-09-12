using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new
{
    public class CustomImage : Image
    {
        private bool _materialCreated;
        
        public void SetAvailable(bool available)
        {
            material.SetFloat("_Grayscale", available ? 0f : 1f);
        }
        
        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                material = new Material(material);
                _materialCreated = true;
            }

            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (_materialCreated)
            {
                Destroy(material);
            }
            base.OnDestroy();
        }
    }
}
