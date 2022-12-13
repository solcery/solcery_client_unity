using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions.Animation.Empty;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.Animation.Factory
{
    public sealed class UpdateActionAnimationFactory : IUpdateActionAnimationFactory
    {
        private readonly IGame _game;
        private readonly Dictionary<UpdateActionAnimationTypes, Func<IUpdateActionAnimationFactory, int, JObject, UpdateActionAnimation>> _creationFuncs;
        
        public static IUpdateActionAnimationFactory Create(IGame game)
        {
            return new UpdateActionAnimationFactory(game);
        }
        
        private UpdateActionAnimationFactory(IGame game)
        {
            _game = game;
            _creationFuncs = new Dictionary<UpdateActionAnimationTypes, Func<IUpdateActionAnimationFactory, int, JObject, UpdateActionAnimation>>();
        }
        
        IUpdateActionAnimationFactory IUpdateActionAnimationFactory.RegistrationCreationFunc(UpdateActionAnimationTypes animationType, Func<IUpdateActionAnimationFactory, int, JObject, UpdateActionAnimation> creationFunc)
        {
            if (_creationFuncs.ContainsKey(animationType))
            {
                throw new Exception($"UpdateActionAnimationFactory.RegistrationActionCreator double registration for type {animationType}");
            }
            
            _creationFuncs.Add(animationType, creationFunc);
            return this;
        }
        
        // {
        //     "id": 1,
        //     "state_id": 0,
        //     "action_type": 2,
        //     "value": {
        //         ...
        //         "animation_id": 596
        //     }
        // }
        UpdateActionAnimation IUpdateActionAnimationFactory.CreateAction(int stateId, JObject value)
        {
            if (value.TryGetValue("animation_id", out int animationId)
                && _game.ServiceGameContent.Animations.TryTypeForId(animationId, out var animationType)
                && _creationFuncs.TryGetValue(animationType, out var creationFunc))
            {
                return creationFunc.Invoke(this, stateId, value);
            }
            
            return UpdateActionAnimationEmpty.Create();
        }

        public void Cleanup()
        {
            _creationFuncs.Clear();
        }
    }
}