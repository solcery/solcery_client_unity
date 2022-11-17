namespace Solcery.Models.Shared.Attributes.Values
{
    public sealed class AttributeValue : IAttributeValue
    {
        int IAttributeValue.Old => _old;
        int IAttributeValue.Current => _current;
        bool IAttributeValue.Changed => _changed;
        
        private int _old;
        private int _current;
        private bool _changed;
        private bool _init;

        public static IAttributeValue Create()
        {
            return new AttributeValue();
        }

        public static IAttributeValue Create(int value)
        {
            return new AttributeValue(value);
        }

        private AttributeValue()
        {
            _init = false;
            _changed = false;
        }

        private AttributeValue(int value)
        {
            _init = true;
            _changed = true;
            _current = value;
            _old = value;
        }

        void IAttributeValue.UpdateValue(int value)
        {
            if (!_init)
            {
                _init = true;
                _changed = true;
                _current = value;
                _old = value;
                return;
            }

            _old = _current;
            _current = value;
            _changed = _old != _current;
        }

        void IAttributeValue.ConsumeChanged()
        {
            _changed = false;
        }

        void IAttributeValue.Cleanup()
        {
            _init = false;
            _changed = false;
            _current = -1;
            _old = -1;
        }
    }
}