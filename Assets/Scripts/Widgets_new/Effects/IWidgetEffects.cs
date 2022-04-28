using System;
using Solcery.Games;
using Solcery.Services.Renderer.Widgets;
using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;

namespace Solcery.Widgets_new.Effects
{
    public interface IWidgetEffects
    {
        public void DestroyEclipseCard(IEclipseCardInContainerWidget eclipseCard,
            IWidgetRenderData renderData,
            float time,
            Action onMoveComplete);
        
        public void MoveToken(RectTransform rectTransform,
            Sprite sprite,
            Vector3 from,
            float time,
            Action onMoveComplete);

        public void DestroyToken(RectTransform rectTransform,
            Sprite sprite,
            float time,
            Action onMoveComplete);
        void Destroy();
    }
}