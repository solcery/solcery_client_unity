using Solcery.Models;
using UnityEngine;

namespace Solcery.Services.Widget
{
    public class WidgetService : IWidgetService
    {
        private readonly IWidgetsProvider _widgetsProvider;
        private readonly IModel _model;
        private readonly PoolProvider _poolProvider;
        
        public static WidgetService Create(IWidgetsProvider widgetsProvider, IModel model)
        {
            return new WidgetService(widgetsProvider, model);
        }

        private WidgetService(IWidgetsProvider widgetsProvider, IModel model)
        {
            _widgetsProvider = widgetsProvider;
            _model = model;
            _poolProvider = new PoolProvider("UiPool");
        }

        public UiBaseWidget GetUiWidget(UiWidgetTypes type, UiBaseWidget parent)
        {
            var gameObject = _widgetsProvider.GetUiWidget(type);
            if (gameObject != null)
            {
                return _poolProvider.GetFromPool<UiBaseWidget>(gameObject, parent);
            }
            Debug.LogError($"Can't get ui widget for type {type}");
            return null;
        }

        public void ReturnUiWidget(UiBaseWidget widget)
        {
            _poolProvider.ReturnToPool(widget);
        }

        public void Init()
        {
            _poolProvider.Init(_model.World);
        }

        public void Cleanup()
        {
            _poolProvider.ReturnAllToPool();
        }

        public void Destroy()
        {
            _poolProvider.Dispose();
        }
    }
}
