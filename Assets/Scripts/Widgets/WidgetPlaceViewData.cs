using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Widgets.Deck
{
    public class WidgetPlaceViewData : WidgetViewDataBase
    {
        private const float AnchorDivider = 10000.0f;
        public JObject RawData { get; private set; }
        public int ZOrder { get; private set; }
        public Vector2 AnchorMin { get; private set; }
        public Vector2 AnchorMax { get; private set; }
        public bool Visible { get; private set; }
        public bool Face { get; private set; }

        public override bool TryParse(JObject jsonData)
        {
            base.TryParse(jsonData);
            
            if (!jsonData.TryGetValue("x1", out var x1) ||
                !jsonData.TryGetValue("y1", out var y1) || 
                !jsonData.TryGetValue("x2", out var x2) || 
                !jsonData.TryGetValue("y2", out var y2) || 
                !jsonData.TryGetValue("zOrder", out var zOrder))
            {
                return false;
            }

            RawData = jsonData;
            AnchorMin = new Vector2(x1.Value<int>() / AnchorDivider, y1.Value<int>() / AnchorDivider);
            AnchorMax = new Vector2(x2.Value<int>() / AnchorDivider, y2.Value<int>() / AnchorDivider);
            ZOrder = zOrder.Value<int>();
            
            if (jsonData.TryGetValue("visible", out var visible))
            {
                Visible = visible.Value<bool>();
            }
            
            if (jsonData.TryGetValue("face", out var face))
            {
                Face = face.Value<bool>();
            }
            return true;
        }
    }
}