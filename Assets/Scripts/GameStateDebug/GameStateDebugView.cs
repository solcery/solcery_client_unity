#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.GameStateDebug
{
    public sealed class GameStateDebugView : MonoBehaviour
    {
        [SerializeField]
        public JObject GameState;
    }
}
#endif