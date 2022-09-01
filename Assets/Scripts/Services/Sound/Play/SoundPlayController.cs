using System;
using DG.Tweening;
using UnityEngine;

namespace Solcery.Services.Sound.Play
{
    public sealed class SoundPlayController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        public Action<SoundPlayController> OnPlayFinished;

        private Tween _delay;
        private bool _isPlaying;

        public void Play(AudioClip clip)
        {
            gameObject.SetActive(true);
            audioSource.clip = clip;
            
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }

            _delay = DOTween.To(value => { }, 0f, 0f, clip.length + 0.1f);
            _delay.SetRecyclable(true);
            _delay.OnComplete(OnPlayFinish);
            _delay.OnKill(OnPlayFinish);
            _isPlaying = true;
        }

        private void OnPlayFinish()
        {
            if (!_isPlaying)
            {
                return;
            }
            
            _delay.OnComplete(null);
            _delay.OnKill(null);
            _delay = null;
            _isPlaying = false;
            OnPlayFinished?.Invoke(this);
        }

        public void Cleanup()
        {
            if (_isPlaying)
            {
                _delay?.Kill(true);    
            }

            audioSource.clip = null;
            gameObject.SetActive(false);
        }
    }
}