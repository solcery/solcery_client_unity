namespace Solcery.Widgets_new.Container
{
    public class PlaceWidgetContainerLayout : PlaceWidgetLayout
    {
        public override void UpdateAlpha(int alpha)
        {
            if (canvasGroup == null)
                return;

            var isAlphaZero = alpha == 0;
            canvasGroup.interactable = !isAlphaZero;
            canvasGroup.blocksRaycasts = !isAlphaZero;

            var a = alpha / 100f;
            canvasGroup.alpha = a;
        }
    }
}