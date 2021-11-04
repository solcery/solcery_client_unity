using Solcery.Games;
using Solcery.Services.Widget;
using UnityEngine;

namespace Solcery
{
    public class GameApplication : MonoBehaviour
    {
        [SerializeField]
        public UiWidgetSettings _uiWidgetSettings;
        
        private IGame _game;

        private static GameApplication _instance;
        
        public static IGame Game()
        {
            return _instance._game;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        private void Start()
        {
            _game = Games.Game.Create(_uiWidgetSettings);
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