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
        private TMP_Text timerText;

        public void UpdateTimer(int timer)
        {
            timerText.text = timer.ToString();
        }

        public void Cleanup()
        {
        }
    }
}