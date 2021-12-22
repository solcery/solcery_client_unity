using UnityEngine;

namespace Solcery.React
{
    public class ReactDontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}