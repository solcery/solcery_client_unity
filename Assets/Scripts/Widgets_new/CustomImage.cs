using System;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new
{
    public class CustomImage : Image
    {
        private Material _material;
        public static readonly int GrayscaleId = Shader.PropertyToID("_Grayscale");
        
        public override Material materialForRendering
        {
            get
            {
                var baseMaterial = base.materialForRendering;
                baseMaterial.SetFloat(GrayscaleId, _isAvailable ? 0f : 1f);
                return baseMaterial;
            }
        }

        private bool _isAvailable = true;
        private bool _isUpdated = true;
        
        public void SetAvailable(bool available)
        {
            _isUpdated = _isAvailable != available;
            _isAvailable = available;
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

        public void LateUpdate()
        {
            if (_isUpdated)
            {
                gameObject.SetActive(false);
                gameObject.SetActive(true);
                _isUpdated = false;
            }
        }
    }
}
