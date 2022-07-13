using UnityEngine;

namespace Solcery.Widgets_new.Timer
{
    public sealed class PlaceWidgetTimer : IPlaceWidgetTimer
    {
        private int _startTimerDurationMsec;
        private readonly PlaceWidgetTimerLayout _timerLayout;

        public static IPlaceWidgetTimer Create(PlaceWidgetTimerLayout timerLayout)
        {
            return new PlaceWidgetTimer(timerLayout);
        }

        private PlaceWidgetTimer(PlaceWidgetTimerLayout timerLayout)
        {
            _timerLayout = timerLayout;
            timerLayout.timerImage.color = timerLayout.timerImageColor;
        }

        public void Start(int durationMsec)
        {
            if (_timerLayout == null)
            {
                return;
            }
            
            _startTimerDurationMsec = durationMsec;
            _timerLayout.timerFrame.gameObject.SetActive(true);
            _timerLayout.timerFrame.fillAmount = 1f;
            
            var color = _timerLayout.timerImage.color;
            color.a = _timerLayout.timerImageAlpha.Evaluate(0f);
            _timerLayout.timerImage.color = color;
        }

        public void Update(int durationMsec)
        {
            if (_timerLayout == null)
            {
                return;
            }
            
            var f = durationMsec / (float) _startTimerDurationMsec; 
            _timerLayout.timerFrame.fillAmount = Mathf.Max(0f, Mathf.Min(1f, f));
            
            var alpha = _timerLayout.timerImageAlpha.Evaluate(1f - f);
            if (durationMsec <= _timerLayout.timerStartFlashingTimeMsec)
            {
                var tb = _timerLayout.timerFlashingPeriodAcceleration.Evaluate(1f - durationMsec / 20000f);
                var ct = (int)(_timerLayout.timerFlashingPeriodMsec * tb);
                var t = durationMsec % ct / (float)ct;
                var evoT = Mathf.Max(0f, Mathf.Min(1f, t));
                alpha *= _timerLayout.timerFlashingAlpha.Evaluate(evoT);
            }
            
            var color = _timerLayout.timerImage.color;
            color.a = alpha;
            _timerLayout.timerImage.color = color;
        }

        public void Stop()
        {
            if (_timerLayout == null)
            {
                return;
            }
            
            _timerLayout.timerFrame.gameObject.SetActive(false);
        }
    }
}