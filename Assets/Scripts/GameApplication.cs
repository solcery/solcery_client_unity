using Solcery.Games;
using Solcery.Ui.DragDrop;
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
        private RootDragDropLayout dragDropCanvas;
        [SerializeField]
        private Camera rootCamera;
        
        private IGame _game;

        private static GameApplication _instance;
        public static GameApplication Instance => _instance;
        
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

        public Vector3 WorldToCanvas(Vector3 worldPosition)
        {
            var dX = worldPosition.x / Screen.width;
            var dY = worldPosition.y / Screen.height;
            var rect = uiCanvas.rect;
            return new Vector3(rect.width * dX, rect.height * dY);
        }
    }
}