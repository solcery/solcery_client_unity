using UnityEngine;

namespace Solcery.Services.Sound.Play
{
    public sealed class SoundPlayController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        public void SetAudioClip(AudioClip clip)
        {
            gameObject.SetActive(true);
            audioSource.clip = clip;
        }

        public void Play()
        {
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }
        }

        public void Cleanup()
        {
            audioSource.clip = null;
            gameObject.SetActive(false);
        }
    }
}