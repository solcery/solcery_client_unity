using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Attributes.Number
{
    public struct ComponentAttributeNumber : IEcsAutoReset<ComponentAttributeNumber>
    {
        public int Value;

        public void AutoReset(ref ComponentAttributeNumber c)
        {
            c.Value = -1;
        }
    }
}