using Solcery.Models;
using UnityEngine;

namespace Solcery.Services.Widget
{
    public class WidgetService : IWidgetService
    {
        private readonly IWidgetsProvider _widgetsProvider;
        private readonly IModel _model;
        
        public static WidgetService Create(IWidgetsProvider widgetsProvider, IModel model)
        {
            return new WidgetService(widgetsProvider, model);
        }

        private WidgetService(IWidgetsProvider widgetsProvider, IModel model)
        {
            _widgetsProvider = widgetsProvider;
            _model = model;
        }

        public UiBaseWidget GetUiWidget(UiWidgetTypes type, UiBaseWidget parent)
        {
            var gameObject = _widgetsProvider.GetUiWidget(type);
            if (gameObject != null)
            {
                var widget = Object.Instantiate(gameObject).GetComponent<UiBaseWidget>();
                widget.Init(_model.World, parent);
                return widget;
            }
            else
            {
                Debug.LogError($"Can't get ui widget for type {type}");
                return null;
            }
        }

        public void ReturnUiWidget(UiBaseWidget widget)
        {
            widget.Clear();
            // todo will support pooling
        }

        public void Init()
        {
        }

        public void Cleanup()
        {
        }

        public void Destroy()
        {
        }
    }
}
