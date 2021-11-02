using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models;
using Solcery.Services.GameContent;
using Solcery.Services.Transport;

namespace Solcery.Games
{
    public class Game : IGame
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
            InitServices();
            
            _model = PlayModel.Create();
        }

        private void CreateServices()
        {
#if UNITY_EDITOR
            _transportService = EditorTransportService.Create();
#else
            _transportService = WebGlTransportService.Create();
#endif
            
            _brickService = BrickService.Create();
            _gameContentService = GameContentService.Create();
        }

        private void InitServices()
        {
            RegistrationBrickTypes();
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

        void IGame.Update(float dt)
        {
            throw new System.NotImplementedException();
        }

        void IGame.Destroy()
        {
            throw new System.NotImplementedException();
        }

    }
}