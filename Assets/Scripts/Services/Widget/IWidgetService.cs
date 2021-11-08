using Solcery.Widgets.UI;

namespace Solcery.Services.Widget
{
    public interface IWidgetService
    {
        public UiBaseWidget GetUiWidget(UiWidgetTypes type, UiBaseWidget parent);
        public void ReturnUiWidget(UiBaseWidget widget);
        void Init();
        void Cleanup();
        void Destroy();    
    }
}
