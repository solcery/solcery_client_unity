using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.DebugViewers.States
{
    public abstract class DebugState
    {
        public readonly int StateIndex;

        protected DebugState(int stateIndex)
        {
            StateIndex = stateIndex;
        }

        public abstract void Draw(RectTransform content, JObject parameters);
        public abstract void Cleanup();
    }

    public abstract class DebugState<T> : DebugState  where T : MonoBehaviour
    {
        protected T Layout;

        protected DebugState(int stateIndex) : base(stateIndex) { }
    }
}