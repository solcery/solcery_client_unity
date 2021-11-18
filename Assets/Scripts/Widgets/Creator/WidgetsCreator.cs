using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Creator
{
    public class WidgetsCreator : IWidgetsCreator
    {
        private readonly List<IWidgetCreator> _widgets;

        public static WidgetsCreator Create()
        {
            return new WidgetsCreator();
        }

        private WidgetsCreator()
        {
            _widgets = new List<IWidgetCreator>();
        }

        public void Register(IWidgetCreator creator)
        {
            _widgets.Add(creator);
        }

        public Widget CreateWidget(JObject data)
        {
            foreach (var creator in _widgets)
            {
                if (creator.TryCreate(data, out var widget))
                {
                    return widget;
                }
            }

            return null;
        }        
    }
}