using UnityEngine;

namespace Solcery.Widgets_new
{
    public class CustomImages : MonoBehaviour
    {
        private CustomImage[] _images;

        public void Awake()
        {
            _images = GetComponentsInChildren<CustomImage>();
        }
        
        public void UpdateAvailable(bool available)
        {
            foreach (var image in _images)
            {
                image.SetAvailable(available);
            }
        }    
    }
}
