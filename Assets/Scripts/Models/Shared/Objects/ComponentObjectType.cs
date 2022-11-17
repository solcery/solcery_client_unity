using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectType : IEcsAutoReset<ComponentObjectType>
    {
        public int TplId
        {
            get => _tplId;

            set
            {
                _changed = _tplId != value;
                _tplId = value;
            }
        }
        
        public bool Changed => _changed;

        public void ConsumeChanged()
        {
            _changed = false;
        }

        private int _tplId;
        private bool _changed;

        public void AutoReset(ref ComponentObjectType c)
        {
            c._tplId = -1;
            c._changed = false;
        }
    }
}