using Leopotam.EcsLite;

namespace Solcery.Models.Play.Attributes.Place
{
    public struct ComponentAttributePlace : IEcsAutoReset<ComponentAttributePlace>
    {
        public int Value;
        
        public void AutoReset(ref ComponentAttributePlace c)
        {
            c.Value = -1;
        }
    }
}