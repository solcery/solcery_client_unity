using Solcery.Games;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery
{
    public class GameApplication : MonoBehaviour
    {
        [SerializeField]
        private Transform gameCanvas;
        [SerializeField]
        private RectTransform uiCanvas;
        [SerializeField]
        private RectTransform dragDropCanvas;
        
        private IGame _game;

        private static GameApplication _instance;
        
        public static IGame Game()
        {
            return _instance._game;
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _game = Games.Game.Create(WidgetCanvas.Create(gameCanvas, uiCanvas, dragDropCanvas));
            _game.Init();
        }

        private void Update()
        {
            _game?.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _game?.Destroy();
            _game = null;
        }
    }
}