namespace Solcery.Widgets_new.Simple
{
    public class PlaceWidgetSimpleLayout : PlaceWidgetLayout
    {
        public override void UpdateAlpha(int alpha)
        {
            if (canvasGroup == null)
            {
                return;
            }

            var a = alpha / 100f;
            canvasGroup.alpha = a;
        }
    }
}