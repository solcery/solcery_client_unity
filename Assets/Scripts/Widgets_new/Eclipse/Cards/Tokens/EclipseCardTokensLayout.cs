using System.Collections.Generic;
using UnityEngine;

namespace Solcery.Widgets_new.Eclipse.Cards.Tokens
{
    public sealed class EclipseCardTokensLayout : MonoBehaviour
    {
        [SerializeField]
        private List<EclipseCardTokenLayout> tokens;

        private int _tokenSlots;
        
        public void UpdateTokenSlots(int count)
        {
            _tokenSlots = tokens.Count > count ? count : tokens.Count;
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i].Cleanup();
                tokens[i].gameObject.SetActive(i < count);
            }
        }
        
        public EclipseCardTokenLayout GetTokenByIndex(int index)
        {
            return index >= 0 && index < _tokenSlots ? tokens[index] : null;
        }

        public void Cleanup()
        {
            _tokenSlots = 0;
            foreach (var token in tokens)
            {
                token.Cleanup();
            }
        }
    }
}