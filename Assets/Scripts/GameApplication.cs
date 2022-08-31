using Solcery.Games;
using Solcery.Games.DTO;
using Solcery.Services.Renderer.DTO;
using Solcery.Services.Sound;
using Solcery.Ui;
using Solcery.Ui.DragDrop;
using Solcery.Widgets_new.Canvas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery
{
    public class GameApplication : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timer;
        [SerializeField]
        private Transform worldGame;
        [SerializeField]
        private RootUiGame uiGame;
        [SerializeField]
        private RootDragDropLayout dragDropCanvas;
        [SerializeField]
        private Camera rootCamera;
        [SerializeField]
        private Transform renderFrame;
        [SerializeField]
        private GameObject renderPrefab;
        [SerializeField]
        private Graphic raycastBlockTouches;
        [SerializeField]
        private SoundsLayout soundsLayout;
        
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
            #if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = false;
            #endif
            _game = Games.Game.Create(GameInitDto.Create(rootCamera,
                WidgetCanvas.Create(timer, worldGame, uiGame, dragDropCanvas),
                ServiceRenderDto.Create(renderFrame, renderPrefab),
                soundsLayout));
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
            var rect = uiGame.Game.rect;
            return new Vector3(rect.width * dX, rect.height * dY);
        }

        public void EnableBlockTouches(bool enable)
        {
            raycastBlockTouches.raycastTarget = enable;
        }
    }
}