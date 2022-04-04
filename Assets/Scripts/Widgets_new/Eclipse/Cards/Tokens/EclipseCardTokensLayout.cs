using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Cards.Tokens
{
    public sealed class EclipseCardTokensLayout : MonoBehaviour
    {
        [SerializeField]
        private List<EclipseCardTokenLayout> tokens;
        
        public void UpdateTokenSlots(int count)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i].gameObject.SetActive(i < count);
            }
        }
        
        public EclipseCardTokenLayout GetTokenByIndex(int index)
        {
            return index >= 0 && index < tokens.Count ? tokens[index] : null;
        }

        public void Cleanup()
        {
            foreach (var token in tokens)
            {
                token.Cleanup();
            }
        }
    }
}