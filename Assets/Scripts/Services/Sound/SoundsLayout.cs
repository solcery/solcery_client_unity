using Solcery.Services.Sound.Play;
using UnityEngine;

namespace Solcery.Services.Sound
{
    public sealed class SoundsLayout : MonoBehaviour
    {
        [SerializeField]
        private GameObject soundPlayPrefab;

        public SoundPlayController CreateSoundPlayController()
        {
            var obj = GameObject.Instantiate(soundPlayPrefab, transform);
            return obj.GetComponent<SoundPlayController>();
        }
    }
}