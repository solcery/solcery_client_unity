using Solcery.Services.Widget.Data;
using Solcery.Services.Widget.Pool;
using Solcery.Widgets.Attributes;
using UnityEngine;

namespace Solcery.Widgets.UI
{
    public class UiBaseWidget : PoolObject, IVisible
    {
        public RectTransform RectTransform;

        public virtual void SetVisible(bool value)
        {
            gameObject.gameObject.SetActive(value);
        }

        public void ApplyTransform(TransformData transformData)
        {
            RectTransform.position = transformData.Position;
            RectTransform.rotation = Quaternion.Euler(transformData.Rotation);
            RectTransform.localScale = transformData.Scale;
        }
    }
}
