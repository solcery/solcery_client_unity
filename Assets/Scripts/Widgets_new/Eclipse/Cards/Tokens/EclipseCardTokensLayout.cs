using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Cards.Tokens
{
    public sealed class EclipseCardTokensLayout : MonoBehaviour
    {
        [SerializeField]
        private List<EclipseCardTokenLayout> tokens;
        
        public EclipseCardTokenLayout GetTokenByIndex(int index)
        {
            return index < tokens.Count ? tokens[index] : null;
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