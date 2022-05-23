using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Cards.Timers
{
    public sealed class EclipseCardTimerLayout : MonoBehaviour
    {
        [SerializeField]
        private Image timerIcon;
        [SerializeField]
        private TMP_Text timerValue;
        [SerializeField]
        private TMP_Text timerText;

        public void UpdateTimerValue(int value)
        {
            timerValue.text = value.ToString();
        }

        public void UpdateTimerText(string text)
        {
            timerText.text = text;
        }
        
        public void UpdateTimerTextActive(bool active)
        {
            timerText.gameObject.SetActive(active);
        }
        
        public void Cleanup()
        {
        }
    }
}