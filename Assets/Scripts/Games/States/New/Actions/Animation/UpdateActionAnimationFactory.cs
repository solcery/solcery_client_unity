using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.Animation
{
    public sealed class UpdateActionAnimationFactory
    {
        private readonly IGame _game;
        private readonly Dictionary<UpdateActionAnimationTypes, Func<JObject, UpdateAction>> _creationFuncs;
        
        public static UpdateActionAnimationFactory Create(IGame game)
        {
            return new UpdateActionAnimationFactory(game);
        }
        
        private UpdateActionAnimationFactory(IGame game)
        {
            _game = game;
            _creationFuncs = new Dictionary<UpdateActionAnimationTypes, Func<JObject, UpdateAction>>();
        }
        
        public UpdateActionAnimationFactory RegistrationCreationFunc(UpdateActionAnimationTypes animationType, Func<JObject, UpdateAction> creationFunc)
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
        public UpdateAction CreateAction(JObject data)
        {
            if (data.TryGetValue("value", out JObject value)
                && value.TryGetValue("animation_id", out int animationId)
                && _game.ServiceGameContent.Animations.TryTypeForId(animationId, out var animationType)
                && _creationFuncs.TryGetValue(animationType, out var creationFunc))
            {
                return creationFunc.Invoke(data);
            }
            
            return UpdateActionAnimationEmpty.Create();
        }

        public void Cleanup()
        {
            _creationFuncs.Clear();
        }
    }
}