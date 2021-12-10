using Leopotam.EcsLite;
using Solcery.Models.Shared.Attributes.Values;

namespace Solcery.Models.Shared.Attributes.Interactable
{
    public struct ComponentAttributeInteractable : IEcsAutoReset<ComponentAttributeInteractable>
    {
        private IAttributeValue _value;

        public bool Value => _value.Current == 1;

        public void Update(int value)
        {
            _value.UpdateValue(value);
        }
        
        public void AutoReset(ref ComponentAttributeInteractable c)
        {
            c._value ??= AttributeValue.Create();
            c._value.Cleanup();
        }
    }
}