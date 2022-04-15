using Leopotam.EcsLite;
using Solcery.Widgets_new.Eclipse.Cards;

namespace Solcery.Models.Play.Eclipse
{
    public struct ComponentEclipseCardView : IEcsAutoReset<ComponentEclipseCardView>
    {
        public IEclipseCardInContainerWidget View;
        
        public void AutoReset(ref ComponentEclipseCardView c)
        {
            c.View = null;
        }
    }
}