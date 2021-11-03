using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models;
using Solcery.Services.GameContent;
using Solcery.Services.Transport;
using UnityEngine;

namespace Solcery.Games
{
    public class Game : IGame, IGameOnReceivingGameContent
    {
        ITransportService IGame.TransportService => _transportService;
        IBrickService IGame.BrickService => _brickService;
        IGameContentService IGame.GameContentService => _gameContentService;
        IModel IGame.Model => _model;
        
        private ITransportService _transportService;
        private IBrickService _brickService;
        private IModel _model;
        private IGameContentService _gameContentService;

        public static IGame Create()
        {
            return new Game();
        }
        
        private Game() { }
        
        void IGame.Init()
        {
            CreateServices();
            CreateModel();
            _transportService.CallUnityLoaded();
        }

        private void CreateServices()
        {
#if UNITY_EDITOR
            _transportService = EditorTransportService.Create(this);
#else
            _transportService = WebGlTransportService.Create(this);
#endif
            
            _brickService = BrickService.Create();
            _gameContentService = GameContentService.Create(_transportService);
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
            _model.Init();
            RegistrationBrickTypes();
            _gameContentService.Init();
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
            _gameContentService.Cleanup();
            _brickService.Cleanup();
            _transportService.Cleanup();
        }

        void IGame.Update(float dt)
        {
            _model.Update(dt);
        }

        void IGame.Destroy()
        {
            _model.Destroy();
            _model = null;
            _gameContentService.Destroy();
            _gameContentService = null;
            _brickService.Destroy();
            _brickService = null;
            _transportService.Destroy();
            _transportService = null;
        }

    }
}