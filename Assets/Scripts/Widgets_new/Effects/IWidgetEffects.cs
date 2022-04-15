using System;
using UnityEngine;

namespace Solcery.Widgets_new.Effects
{
    public interface IWidgetEffects
    {
        public void MoveToken(Sprite sprite,
            Vector2 sizeDelta,
            Vector3 from,
            Vector3 to,
            float time,
            Action onMoveComplete);

        public void DestroyToken(Sprite sprite,
            Vector2 sizeDelta,
            Vector3 position,
            float time,
            Action onMoveComplete);
        void Destroy();
    }
}