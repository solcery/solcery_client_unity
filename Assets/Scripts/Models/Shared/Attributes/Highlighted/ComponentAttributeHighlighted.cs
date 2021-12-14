using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Attributes.Highlighted
{
    public struct ComponentAttributeHighlighted : IEcsAutoReset<ComponentAttributeHighlighted>
    {
        private IAttributeValue _value;

        public bool Value => _value.Current == 1;

        public void Update(int value)
        {
            _value.UpdateValue(value);
        }
        
        public void AutoReset(ref ComponentAttributeHighlighted c)
        {
            c._value ??= AttributeValue.Create();
            c._value.Cleanup();
        }
    }
}