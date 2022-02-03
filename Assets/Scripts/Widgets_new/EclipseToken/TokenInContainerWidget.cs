using Solcery.Games;
using UnityEngine;

namespace Solcery.Widgets_new.EclipseToken
{
    public class TokenInContainerWidget : ITokenInContainerWidget
    {
        private IGame _game;
        private TokenInContainerWidgetLayout _layout;
        
        public static ITokenInContainerWidget Create(IGame game, GameObject prefab, Transform poolTransform)
        {
            return new TokenInContainerWidget(game, prefab, poolTransform);
        }
        
        private TokenInContainerWidget(IGame game, GameObject prefab, Transform poolTransform)
        {
            _game = game;
            _layout = Object.Instantiate(prefab, poolTransform).GetComponent<TokenInContainerWidgetLayout>();
        }
        
        void ITokenInContainerWidget.Cleanup()
        {
            Cleanup();
            _layout.Cleanup();
        }

        void ITokenInContainerWidget.Destroy()
        {
            Cleanup();
            _layout.Cleanup();
            Object.Destroy(_layout.gameObject);
            _layout = null;

            _game = null;
        }

        private void Cleanup()
        {
        }    
    }
}
