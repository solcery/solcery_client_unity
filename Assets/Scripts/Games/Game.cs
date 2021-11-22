using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models;
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Utils;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Factory;

namespace Solcery.Games
{
    public sealed class Game : IGame, IGameTransportCallbacks, IGameResourcesCallback
    {
        ITransportService IGame.TransportService => _transportService;
        IServiceBricks IGame.ServiceBricks => _serviceBricks;
        IServiceResource IGame.ServiceResource => _serviceResource;
        IModel IGame.Model => _model;
        IWidgetFactory IGame.WidgetFactory => _widgetFactory;

        JObject IGame.GameContent => _gameContentJson;

        JObject IGame.GameStatePopAndClear
        {
            get
            {
                var lastGameContent = _gameStates.Count > 0 ? _gameStates.Pop() : null;
                _gameStates.Clear();
                return lastGameContent;
            }
        }

        private ITransportService _transportService;
        private IServiceBricks _serviceBricks;
        private IServiceResource _serviceResource;
        private IModel _model;
        private IWidgetFactory _widgetFactory;

        private JObject _gameContentJson;
        private Stack<JObject> _gameStates;

        public static IGame Create(IWidgetCanvas widgetCanvas)
        {
            return new Game(widgetCanvas);
        }

        private Game(IWidgetCanvas widgetCanvas)
        {
            _gameStates = new Stack<JObject>();
            CreateModel();
            CreateServices(widgetCanvas);
        }
        
        private void CreateModel()
        {
            _model = PlayModel.Create();
        }
        
        private void CreateServices(IWidgetCanvas widgetCanvas)
        {
            _serviceBricks = ServiceBricks.Create();
            RegistrationBrickTypes();
            
#if UNITY_EDITOR
            _transportService = EditorTransportService.Create(this, _serviceBricks);
#else
            _transportService = WebGlTransportService.Create(this);
#endif

            _serviceResource = ServiceResource.Create(this);
            _widgetFactory = WidgetFactory.Create(widgetCanvas, _serviceResource);
        }

        void IGame.Init()
        {
            _transportService.CallUnityLoaded();
        }

        void IGameTransportCallbacks.OnReceivingGameContent(JObject gameContentJson)
        {
            Cleanup();
            _gameContentJson = gameContentJson;

            if (gameContentJson.TryGetValue("customBricks", out JObject customBricks) &&
                customBricks.TryGetValue("objects", out JArray customBricksArray))
            {
                _serviceBricks.RegistrationCustomBricksData(customBricksArray);
            }
            
            _serviceResource.PreloadResourcesFromGameContent(_gameContentJson);
        }
        
        void IGameResourcesCallback.OnResourcesLoad()
        {
            Init(_gameContentJson);
        }

        void IGameTransportCallbacks.OnReceivingGameState(JObject gameStateJson)
        {
            _gameStates.Push(gameStateJson);
        }

        private void Init(JObject gameContentJson)
        {
            _model.Init(this, gameContentJson);
        }

        private void RegistrationBrickTypes()
        {
            // Value bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Const, BrickValueConst.Create);
            // Action bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Void, BrickActionVoid.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Set, BrickActionSet.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Conditional, BrickActionConditional.Create);
            // Condition bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Const, BrickConditionConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Not, BrickConditionNot.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.And, BrickConditionAnd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Or, BrickConditionOr.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Equal, BrickConditionEqual.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.GreaterThan, BrickConditionGreaterThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.LesserThan, BrickConditionLesserThan.Create);
        }

        private void Cleanup()
        {
            _model.Destroy();
            _serviceBricks.Cleanup();
            _transportService.Cleanup();
            _widgetFactory.Cleanup();
            _serviceResource.Cleanup();
        }

        void IGame.Update(float dt)
        {
            _model.Update(dt);
        }

        void IGame.Destroy()
        {
            _model.Destroy();
            _model = null;
            
            _serviceBricks.Destroy();
            _serviceBricks = null;
            _transportService.Destroy();
            _transportService = null;

            // TODO: удаляем последними, так как в разных объектах могут быть ссылки на виджеты и ресурсы
            _widgetFactory.Destroy();
            _widgetFactory = null;
            _serviceResource.Destroy();
            _serviceResource = null;
        }
    }
}