using System;
using Solcery.Games;
using Solcery.Widgets_new.Eclipse.Cards;
using UnityEngine;

namespace Solcery.Widgets_new.Effects
{
    public interface IWidgetEffects
    {
        public void DestroyEclipseCard(RectTransform rectTransform,
            RenderTexture rtt,
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