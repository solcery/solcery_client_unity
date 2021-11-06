using UnityEngine;

namespace Solcery.Services.Widget
{
    public interface IWidgetsProvider
    {
        public GameObject GetUiWidget(UiWidgetTypes type);
    }
}