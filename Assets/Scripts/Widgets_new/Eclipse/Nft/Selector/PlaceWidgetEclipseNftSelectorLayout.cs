using Solcery.Widgets_new.Eclipse.Nft.Card;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Nft.Selector
{
    public sealed class PlaceWidgetEclipseNftSelectorLayout : PlaceWidgetLayout
    {
        [SerializeField]
        private ScrollRect scroll;

        [SerializeField]
        private HorizontalLayoutGroup horizontalLayoutGroup;
        
        public void SetAnchor(TextAnchor anchor)
        {
            horizontalLayoutGroup.childAlignment = anchor;
            switch (anchor)
            {
                case TextAnchor.MiddleLeft:
                    scroll.content.pivot = new Vector2(0f, 1f);
                    break;
                case TextAnchor.MiddleRight:
                    scroll.content.pivot = new Vector2(1f, 0f);
                    break;
                case TextAnchor.MiddleCenter:
                    scroll.content.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }

        public void AddCard(IEclipseCardNftInContainerWidget inContainerWidget)
        {
            inContainerWidget.UpdateParent(scroll.content);
            inContainerWidget.UpdateSiblingIndex(inContainerWidget.Order);
            scroll.horizontal = scroll.content.childCount > 1;
            Rebuild();
        }

        public void Rebuild()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        }
    }
}