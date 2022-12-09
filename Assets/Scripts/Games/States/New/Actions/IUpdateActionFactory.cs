using System;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates.Actions;
using Solcery.Games.States.New.Actions.Animation.Factory;

namespace Solcery.Games.States.New.Actions
{
    public interface IUpdateActionFactory
    {
        void RegistrationCreationFunc(ContextGameStateActionTypes actionType, Func<int, JObject, UpdateAction> creationFunc);
        bool TryGetActionFromJson(JObject actionObject, out UpdateAction action);
        void Cleanup();
    }
}