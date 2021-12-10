using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Attributes.Place
{
    public struct ComponentAttributePlace : IEcsAutoReset<ComponentAttributePlace>
    {
        public IAttributeValue Value;
        
        public void AutoReset(ref ComponentAttributePlace c)
        {
            c.Value ??= AttributeValue.Create();
            c.Value.Cleanup();
        }
    }
}