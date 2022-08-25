using System;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Games.States.New.States;

namespace Solcery.Games.States.New.Factory
{
    public interface IUpdateStateFactory
    {
        void RegistrationCreationFunc(ContextGameStateTypes updateStateType, Func<ContextGameStateTypes, UpdateState> updateStateCreationFunc);
        void Init();
        UpdateState ConstructFromJObject(JObject stateUpdateObject, bool isPredictable);
        void Deconstruct(UpdateState updateState);
    }
}