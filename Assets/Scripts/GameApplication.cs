using Solcery.Games;
using UnityEngine;

namespace Solcery
{
    public class GameApplication : MonoBehaviour
    {
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
            _game = Games.Game.Create();
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