using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models;
using Solcery.Services.GameContent;
using Solcery.Services.GameContent.PrepareData;
using Solcery.Services.GameState;
using Solcery.Services.Transport;
using Solcery.Services.Ui;
using Solcery.Services.Widget;

namespace Solcery.Games
{
    public sealed class Game : IGame, IGameOnReceivingGameContent
    {
        ITransportService IGame.TransportService => _transportService;
        IBrickService IGame.BrickService => _brickService;
        IGameContentService IGame.GameContentService => _gameContentService;
        IGameContentPrepareDataService IGame.GameContentPrepareDataService => _gameContentPrepareDataService;
        IGameStateService IGame.GameStateService => _gameStateService;

        IModel IGame.Model => _model;

        private ITransportService _transportService;
        private IBrickService _brickService;
        private IModel _model;
        private IGameContentService _gameContentService;
        private IGameContentPrepareDataService _gameContentPrepareDataService;
        private IGameStateService _gameStateService;
        private IUiService _uiService;
        private IWidgetService _widgetService;
        private IWidgetsProvider _widgetsProvider;

        public static IGame Create(IWidgetsProvider widgetsProvider)
        {
            return new Game(widgetsProvider);
        }

        private Game(IWidgetsProvider widgetsProvider)
        {
            _widgetsProvider = widgetsProvider;
        }

        void IGame.Init()
        {
            CreateModel();
            CreateServices();
            _transportService.CallUnityLoaded();
        }

        private void CreateServices()
        {
            _brickService = BrickService.Create();
#if UNITY_EDITOR
            _transportService = EditorTransportService.Create(this, _brickService);
#else
            _transportService = WebGlTransportService.Create(this);
#endif
            _gameContentService = GameContentService.Create(_transportService);
            _gameContentPrepareDataService = GameContentPrepareDataService.Create(_gameContentService, _model);
            _gameStateService = GameStateService.Create(_transportService, _model);
            _widgetService = WidgetService.Create(_widgetsProvider, _model);
            _uiService = UiService.Create(_gameContentService, _widgetService, _model);
        }

        private void CreateModel()
        {
            _model = PlayModel.Create();
        }

        void IGameOnReceivingGameContent.OnReceivingGameContent()
        {
            Cleanup();
            Init();
        }

        private void Init()
        {
            _model.Init(this);
            RegistrationBrickTypes();
            _gameContentService.Init();
            _gameContentPrepareDataService.Init();
            _gameStateService.Init();
            _widgetService.Init();
            _uiService.Init();
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
            _gameContentPrepareDataService.Cleanup();
            _gameContentService.Cleanup();
            _brickService.Cleanup();
            _transportService.Cleanup();
            _widgetService.Cleanup();
            _uiService.Cleanup();
        }

        void IGame.Update(float dt)
        {
            _gameContentService.Update();
            _gameStateService.Update();
            _model.Update(dt);
        }

        void IGame.Destroy()
        {
            _model.Destroy();
            _model = null;
            _gameStateService.Destroy();
            _gameStateService = null;
            _gameContentPrepareDataService.Destroy();
            _gameContentPrepareDataService = null;
            _gameContentService.Destroy();
            _gameContentService = null;
            _brickService.Destroy();
            _brickService = null;
            _transportService.Destroy();
            _transportService = null;
            _widgetService.Destroy();
            _widgetService = null;
            _uiService.Destroy();
            _uiService = null;
        }
    }
}