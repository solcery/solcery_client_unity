using System;
using System.Collections.Generic;
using System.Linq;
using Solcery.Widgets.UI;
using UnityEngine;

namespace Solcery.Services.Widget
{
    [CreateAssetMenu(fileName = "UiWidgetSettings", menuName = "Settings/UiWidgetSettings")]
    public class UiWidgetSettings : ScriptableObject, IWidgetsProvider
    {
        [SerializeField]
        public List<UiWidget> _widgets;

        public GameObject GetUiWidget(UiWidgetTypes type)
        {
            var widget = _widgets.FirstOrDefault(x => x.Type == type);
            return widget?.GameObject;
        }
    }
    
    [Serializable]
    public class UiWidget
    {
        public UiWidgetTypes Type;
        public GameObject GameObject;
    }
}
