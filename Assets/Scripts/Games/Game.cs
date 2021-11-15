using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models;
using Solcery.Services.Transport;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Factory;

namespace Solcery.Games
{
    public sealed class Game : IGame, IGameOnReceivingData
    {
        ITransportService IGame.TransportService => _transportService;
        IBrickService IGame.BrickService => _brickService;
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
        private IBrickService _brickService;
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
            _widgetFactory = WidgetFactory.Create(widgetCanvas);
            
            _brickService = BrickService.Create();
            RegistrationBrickTypes();
            
#if UNITY_EDITOR
            _transportService = EditorTransportService.Create(this, _brickService);
#else
            _transportService = WebGlTransportService.Create(this);
#endif
        }

        void IGame.Init()
        {
            _transportService.CallUnityLoaded();
        }

        void IGameOnReceivingData.OnReceivingGameContent(JObject gameContentJson)
        {
            Cleanup();
            _gameContentJson = gameContentJson;
            Init(gameContentJson);
        }

        void IGameOnReceivingData.OnReceivingGameState(JObject gameStateJson)
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
            _brickService.RegistrationBrickType("brick_value_const", BrickValueConst.Create);
            // Action bricks
            _brickService.RegistrationBrickType("brick_action_void", BrickActionVoid.Create);
            _brickService.RegistrationBrickType("brick_action_set", BrickActionSet.Create);
            _brickService.RegistrationBrickType("brick_action_conditional", BrickActionConditional.Create);
            // Condition bricks
            _brickService.RegistrationBrickType("brick_condition_const", BrickConditionConst.Create);
            _brickService.RegistrationBrickType("brick_condition_not", BrickConditionNot.Create);
            _brickService.RegistrationBrickType("brick_condition_and", BrickConditionAnd.Create);
            _brickService.RegistrationBrickType("brick_condition_or", BrickConditionOr.Create);
            _brickService.RegistrationBrickType("brick_condition_equal", BrickConditionEqual.Create);
            _brickService.RegistrationBrickType("brick_condition_greater_than", BrickConditionGreaterThan.Create);
            _brickService.RegistrationBrickType("brick_condition_lesser_than", BrickConditionLesserThan.Create);
        }

        private void Cleanup()
        {
            _model.Destroy();
            _brickService.Cleanup();
            _transportService.Cleanup();
            _widgetFactory.Cleanup();
        }

        void IGame.Update(float dt)
        {
            _model.Update(dt);
        }

        void IGame.Destroy()
        {
            _model.Destroy();
            _model = null;
            
            _brickService.Destroy();
            _brickService = null;
            _transportService.Destroy();
            _transportService = null;
            
            // TODO: удаляем последней, так как в разных объектах могут быть ссылки на виджеты
            _widgetFactory.Destroy();
            _widgetFactory = null;
        }
    }
}