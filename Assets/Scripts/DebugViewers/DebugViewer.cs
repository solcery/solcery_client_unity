using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.DebugViewers.StateQueues;
using Solcery.DebugViewers.States;
using Solcery.DebugViewers.States.Delays;
using Solcery.DebugViewers.States.Games;
using Solcery.DebugViewers.Views.Attrs;
using Solcery.DebugViewers.Views.Objects;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Solcery.DebugViewers
{
    public sealed class DebugViewer : MonoBehaviour, IDebugViewer
    {
        [SerializeField]
        private Button debugShowButton;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private ScrollRect diffScrollView;
        [SerializeField]
        private Button firstButton;
        [SerializeField]
        private Button lastButton;
        [SerializeField]
        private Button previousButton;
        [SerializeField]
        private Button nextButton;
        [SerializeField]
        private Button deltaButton;
        [SerializeField]
        private Button fullButton;
        [SerializeField]
        private TMP_Dropdown moveToObject;
        [SerializeField]
        private TMP_Text states;
        [SerializeField]
        private RectTransform pool;
        [SerializeField]
        private GameObject delayStateViewPrefab;
        [SerializeField]
        private GameObject gameStateViewPrefab;
        [SerializeField]
        private GameObject attrDebugViewPrefab;
        [SerializeField]
        private GameObject objectDebugViewPrefab;
        [SerializeField]
        private GameObject objectAttrDebugViewPrefab;

        public static IDebugViewer Instance => _instance;
        private static IDebugViewer _instance;

        private const int FirsStateIndex = 0;
        private const int LastStateIndex = int.MaxValue;

        private List<DebugStateInfo> _states;
        private List<DebugDelayState> _delayStates;
        private List<DebugGameState> _gameStates;
        private DebugStateViewPool<DebugDelayStateLayout> _debugDelayStateViewPool;
        private DebugStateViewPool<DebugGameStateLayout> _debugGameStateViewPool;
        private DebugStateViewPool<DebugViewAttrLayout> _attrDebugViewPool;
        private DebugStateViewPool<DebugViewObjectLayout> _objectDebugViewPool;
        private DebugStateViewPool<DebugViewAttrLayout> _objectAttrDebugViewPool;
        
        private JObject _deltaParameters;
        private JObject _fullParameters;
        private DebugState _currentState;

        private IDebugUpdateStateQueue _updateStateQueue;

        private void Awake()
        {
            _instance = this;
            
            debugShowButton.onClick.AddListener(Show);
            closeButton.onClick.AddListener(Hide);
            firstButton.onClick.AddListener(First);
            lastButton.onClick.AddListener(Last);
            previousButton.onClick.AddListener(Previous);
            nextButton.onClick.AddListener(Next);
            deltaButton.onClick.AddListener(Delta);
            fullButton.onClick.AddListener(Full);
            moveToObject.onValueChanged.AddListener(OnValueChange);

            _states = new List<DebugStateInfo>();
            _delayStates = new List<DebugDelayState>();
            _gameStates = new List<DebugGameState>();
            _debugDelayStateViewPool = DebugStateViewPool<DebugDelayStateLayout>.Create(pool, delayStateViewPrefab, 10);
            _debugGameStateViewPool = DebugStateViewPool<DebugGameStateLayout>.Create(pool, gameStateViewPrefab, 10);
            _attrDebugViewPool = DebugStateViewPool<DebugViewAttrLayout>.Create(pool, attrDebugViewPrefab, 10);
            _objectDebugViewPool = DebugStateViewPool<DebugViewObjectLayout>.Create(pool, objectDebugViewPrefab, 10);
            _objectAttrDebugViewPool = DebugStateViewPool<DebugViewAttrLayout>.Create(pool, objectAttrDebugViewPrefab, 10);
            
            _deltaParameters = new JObject
            {
                ["state_view_type"] = new JValue((int)DebugStateViewTypes.delta)
            };
            _fullParameters = new JObject
            {
                ["state_view_type"] = new JValue((int)DebugStateViewTypes.full)
            };
            
            _currentState = null;
        }

        private void OnDestroy()
        {
            // TODO: тут все чистим
            _instance = null;
            
            debugShowButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            firstButton.onClick.RemoveAllListeners();
            lastButton.onClick.RemoveAllListeners();
            previousButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            deltaButton.onClick.RemoveAllListeners();
            fullButton.onClick.RemoveAllListeners();
            moveToObject.onValueChanged.RemoveAllListeners();

            _deltaParameters = null;
            _fullParameters = null;
            _currentState?.Cleanup();
            _states.Clear();
            
            _debugDelayStateViewPool.Cleanup();
            _debugGameStateViewPool.Cleanup();
            _attrDebugViewPool.Cleanup();
            _objectDebugViewPool.Cleanup();
            _objectAttrDebugViewPool.Cleanup();
        }

        private void Show()
        {
            root.SetActive(true);
            Last();
        }

        private void Hide()
        {
            _currentState?.Cleanup();
            _currentState = null;
            moveToObject.options = new List<TMP_Dropdown.OptionData>();
            root.SetActive(false);
        }

        private void First()
        {
            var newState = GetDebugStateForIndex(CheckDebugStateIndex(FirsStateIndex));
            ApplyState(newState, _deltaParameters);
        }

        private void Last()
        {
            var newState = GetDebugStateForIndex(CheckDebugStateIndex(LastStateIndex));
            ApplyState(newState, _deltaParameters);
        }

        private void Previous()
        {
            var index = _currentState.StateIndex;
            index -= 1;
            index = CheckDebugStateIndex(index);
            
            var newState = GetDebugStateForIndex(index);
            ApplyState(newState, _deltaParameters);
        }

        private void Next()
        {
            var index = _currentState.StateIndex;
            index += 1;
            index = CheckDebugStateIndex(index);
            
            var newState = GetDebugStateForIndex(index);
            ApplyState(newState, _deltaParameters);
        }

        private void Delta()
        {
            ApplyState(_currentState, _deltaParameters);
        }

        private void Full()
        {
            ApplyState(_currentState, _fullParameters);
        }
        
        // Тут место для перехода к конкретному объекту
        private void OnValueChange(int arg0)
        {
            if (_currentState != null)
            {
                diffScrollView.content.localPosition = _currentState.GetPositionToKeys(moveToObject.options[arg0].text);
            }
        }

        private void ApplyState(DebugState newState, JObject parameters)
        {
            if (newState == null)
            {
                return;
            }

            _currentState?.Cleanup();
            _currentState = newState;
            _currentState.Draw(diffScrollView.content, parameters);
            diffScrollView.normalizedPosition = new Vector2(0, 1);

            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var moveToKey in _currentState.AllMoveToKeys())
            {
                options.Add(new TMP_Dropdown.OptionData(moveToKey));
            }

            moveToObject.options = options;

            states.text = $"STATE {_currentState.StateIndex + 1}";
        }

        private DebugState GetDebugStateForIndex(int index)
        {
            var debugStateInfo = _states[index];

            switch (debugStateInfo.Type)
            {
                case DebugStateTypes.Delay:
                    return _delayStates[debugStateInfo.Index];

                case DebugStateTypes.Game: 
                    return _gameStates[debugStateInfo.Index];
            }

            return null;
        }

        private int CheckDebugStateIndex(int index)
        {
            return Mathf.Max(0, Mathf.Min(_states.Count - 1, index));
        }

        void IDebugViewer.Show()
        {
            Show();
        }
        
        void IDebugViewer.Hide()
        {
            Hide();
        }

        void IDebugViewer.AddGameStatePackage(JObject gameStateJson)
        {
            _updateStateQueue.AddUpdateStates(gameStateJson);
        }

        // void IDebugViewer.AddGameStatePackage(JObject gameStateJson)
        // {
        //     foreach (var state in gameStatePackage.States)
        //     {
        //         switch (state)
        //         {
        //             case GameState gs:
        //                 var previousFullState = _gameStates.Count > 0 ? _gameStates[^1].FullState : null;
        //                 var gameState = DebugGameState.Create(
        //                     _states.Count, 
        //                     previousFullState, 
        //                     gs.GameStateObject,
        //                     diffScrollView.content, 
        //                     _debugGameStateViewPool,
        //                     _attrDebugViewPool,
        //                     _objectDebugViewPool,
        //                     _objectAttrDebugViewPool);
        //                 _states.Add(DebugStateInfo.Create(DebugStateTypes.Game, _gameStates.Count));
        //                 _gameStates.Add(gameState);
        //                 break;
        //             
        //             case PauseState ps:
        //                 var delayState = DebugDelayState.Create(_states.Count, ps.Delay, diffScrollView.content,
        //                     _debugDelayStateViewPool);
        //                 _states.Add(DebugStateInfo.Create(DebugStateTypes.Delay, _delayStates.Count));
        //                 _delayStates.Add(delayState);
        //                 break;
        //         }
        //     }
        // }
    }
}