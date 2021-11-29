using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Context
{
    public struct ComponentContextObject : IEcsAutoReset<ComponentContextObject>
    {
        private Stack<object> _objects;

        public void Push(object @object)
        {
            _objects.Push(@object);
        }

        public bool TryPop<T>(out T @object)
        {
            if (_objects.Count > 0)
            {
                var obj = _objects.Pop();
                if (obj is T objT)
                {
                    @object = objT;
                    return true;
                }
                
                _objects.Push(obj);
            }

            @object = default;
            return false;
        }

        public bool TryPeek<T>(out T @object)
        {
            if (_objects.Count > 0)
            {
                var obj = _objects.Peek();
                if (obj is T objT)
                {
                    @object = objT;
                    return true;
                }
            }

            @object = default;
            return false;
        }
        
        public void AutoReset(ref ComponentContextObject c)
        {
            c._objects ??= new Stack<object>();
            c._objects.Clear();
        }
    }
}