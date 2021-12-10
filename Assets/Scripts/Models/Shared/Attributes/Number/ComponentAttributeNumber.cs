using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Attributes.Number
{
    public struct ComponentAttributeNumber : IEcsAutoReset<ComponentAttributeNumber>
    {
        public IAttributeValue Value;

        public void AutoReset(ref ComponentAttributeNumber c)
        {
            c.Value ??= AttributeValue.Create();
            c.Value.Cleanup();
        }
    }
}