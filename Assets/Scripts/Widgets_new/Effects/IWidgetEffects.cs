using System;
using Solcery.Services.Renderer.Widgets;
using Solcery.Widgets_new.Eclipse.Effects;
using UnityEngine;

namespace Solcery.Widgets_new.Effects
{
    public interface IWidgetEffects
    {
        public void DestroyEclipseCard(EclipseCardEffectLayout effectLayout,
            IWidgetRenderData renderData,
            float time,
            Action onMoveComplete);
        
        public void MoveEclipseCard(RectTransform eclipseCard,
            float time,
            Vector3 from,
            Action onMoveComplete);
        
        public void MoveToken(RectTransform rectTransform,
            Sprite sprite,
            Vector3 from,
            float time,
            Color color,
            Action onMoveComplete);

        public void DestroyToken(RectTransform rectTransform,
            Sprite sprite,
            float time,
            Color color,
            Action onMoveComplete);

        void Destroy();
    }
}