using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Timer
{
    public sealed class PlaceWidgetTimerLayout : MonoBehaviour
    {
        public Image timerFrame;
        public Image timerImage;
        public Color timerImageColor;
        public AnimationCurve timerImageAlpha;
        public int timerStartFlashingTimeMsec;
        public int timerFlashingPeriodMsec;
        public AnimationCurve timerFlashingAlpha;
        public AnimationCurve timerFlashingPeriodAcceleration;
    }
}