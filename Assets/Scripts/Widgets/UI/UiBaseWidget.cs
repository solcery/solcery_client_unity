using Leopotam.EcsLite;
using UnityEngine;

namespace Solcery
{
    public class UiBaseWidget : MonoBehaviour, IVisible
    {
        public RectTransform RectTransform;
        
        protected EcsWorld EcsWorld;
        
        public virtual void Init(EcsWorld world, UiBaseWidget parent)
        {
            EcsWorld = world;
            if (parent != null)
            {
                transform.SetParent(parent.transform);
            }
            // todo delete it after transform will support
            transform.position = Vector3.zero;
        }
        
        public virtual void Clear()
        {
            EcsWorld = null;
        }
        
        public virtual void SetVisible(bool value)
        {
            gameObject.gameObject.SetActive(value);
        }
    }
}
