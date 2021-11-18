using Newtonsoft.Json.Linq;

namespace Solcery.Widgets.Creator
{
    public abstract class WidgetCreatorBase<T> : IWidgetCreator where T : WidgetViewDataBase, new()
    {
        protected readonly T _viewData;

        protected WidgetCreatorBase()
        {
            _viewData = new T();
        }

        public bool TryCreate(JObject jsonData, out Widget widget)
        {
            if (_viewData.TryParse(jsonData))
            {
                widget = Create();
                return true;
            }

            widget = null;
            return false;
        }

        protected abstract Widget Create();
    }
}