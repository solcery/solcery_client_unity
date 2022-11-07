using System.Collections.Generic;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;

namespace Solcery.Games.Contexts
{
    internal class CurrentContextObject : IContextObject
    {
        private readonly Stack<object> _objects;

        private void Push(object @object)
        {
            _objects.Push(@object);
        }

        private bool TryPop<T>(out T @object)
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

        private bool TryPeek<T>(out T @object)
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

        public static IContextObject Create()
        {
            return new CurrentContextObject();
        }
        
        private CurrentContextObject()
        {
            _objects = new Stack<object>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        void IContextObject.Push(object @object)
        {
            Push(@object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IContextObject.TryPop<T>(out T @object)
        {
            return TryPop(out @object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IContextObject.TryPeek<T>(out T @object)
        {
            return TryPeek(out @object);
        }
    }
}