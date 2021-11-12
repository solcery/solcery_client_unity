using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.Widgets.Button;
using Solcery.Widgets.Canvas;

namespace Solcery.Widgets.Factory
{
    public sealed class WidgetFactory : IWidgetFactory
    {
        // TODO: его нужно передать в виджет при создании, для размещения виджета на нужном лейауте
        private IWidgetCanvas _widgetCanvas;

        public static IWidgetFactory Create(IWidgetCanvas widgetCanvas)
        {
            return new WidgetFactory(widgetCanvas);
        }

        private WidgetFactory(IWidgetCanvas widgetCanvas)
        {
            _widgetCanvas = widgetCanvas;
        }
        
        bool IWidgetFactory.TryCreateWidget(JObject jsonData, out Widget widget)
        {
            widget = null;
            
            if (!jsonData.TryGetValue("layout", out int layout))
            {
                return false;
            }

            var widgetType = (WidgetTypes) layout;

            switch (widgetType)
            {
                case WidgetTypes.None:
                    return false;
                case WidgetTypes.Button:
                    widget = new WidgetButton(_widgetCanvas, jsonData);
                    return true;
            }

            return false;
        }

        void IWidgetFactory.Cleanup()
        {
            Cleanup();
        }

        void IWidgetFactory.Destroy()
        {
            // TODO: тут удаляем все что заинитили на создании
            Cleanup();
            _widgetCanvas = null;
        }

        private void Cleanup()
        {
            // TODO: тут нужно будет почистить пулы виджетов
        }
    }
}