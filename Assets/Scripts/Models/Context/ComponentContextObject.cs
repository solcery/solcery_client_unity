using Leopotam.EcsLite;

namespace Solcery.Models.Context
{
    public struct ComponentContextObject : IEcsAutoReset<ComponentContextObject>
    {
        private object _object;

        public void Set(object @object)
        {
            _object = @object;
        }

        public bool TryGet<T>(out T @object)
        {
            if (_object is T objT)
            {
                @object = objT;
                return true;
            }

            @object = default;
            return false;
        }
        
        public void AutoReset(ref ComponentContextObject c)
        {
            c._object = null;
        }
    }
}