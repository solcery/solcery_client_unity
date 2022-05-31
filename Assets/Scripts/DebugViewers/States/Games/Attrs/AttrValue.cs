namespace Solcery.DebugViewers.States.Games.Attrs
{
    public sealed class AttrValue : IAttrValue
    {
        bool IAttrValue.IsChange => _oldValue != _currentValue;
        string IAttrValue.Key => _key;
        int IAttrValue.CurrentValue => _currentValue;
        int IAttrValue.OldValue => _oldValue;

        private readonly string _key;
        private readonly int _currentValue;
        private readonly int _oldValue;

        public static IAttrValue Create(string key, int currentValue, int oldValue)
        {
            return new AttrValue(key, currentValue, oldValue);
        }

        private AttrValue(string key, int currentValue, int oldValue)
        {
            _key = key;
            _currentValue = currentValue;
            _oldValue = oldValue;
        }
    }
}