using System;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new
{
    public class CustomImage : Image
    {
        private Material _material;
        public static readonly int GrayscaleId = Shader.PropertyToID("_Grayscale");
        
        public void SetAvailable(bool available)
        {
            _material.SetFloat(GrayscaleId, available ? 0f : 1f);
        }

        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                _material = Instantiate(material);
                material = _material;
            }

            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
            base.OnDestroy();
        }
    }
}
