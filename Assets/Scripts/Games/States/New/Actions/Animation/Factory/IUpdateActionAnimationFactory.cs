using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Games.States.New.Actions.Animation.Factory
{
    public interface IUpdateActionAnimationFactory
    {
        IUpdateActionAnimationFactory RegistrationCreationFunc(UpdateActionAnimationTypes animationType, Func<IUpdateActionAnimationFactory, int, JObject, UpdateActionAnimation> creationFunc);
        UpdateActionAnimation CreateAction(int stateId, JObject value);
        void Cleanup();
    }
}