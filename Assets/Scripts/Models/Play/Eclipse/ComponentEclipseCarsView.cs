using Leopotam.EcsLite;
using Solcery.Widgets_new.Eclipse.Cards;

namespace Solcery.Models.Play.Eclipse
{
    public struct ComponentEclipseCarsView : IEcsAutoReset<ComponentEclipseCarsView>
    {
        public IEclipseCardInContainerWidget View;
        
        public void AutoReset(ref ComponentEclipseCarsView c)
        {
            c.View = null;
        }
    }
}