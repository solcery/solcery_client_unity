using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates.Actions;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions
{
    public sealed class UpdateActionFactory : IUpdateActionFactory
    {
        private readonly Dictionary<ContextGameStateActionTypes, Func<JObject, UpdateAction>> _creationFuncs;

        public static IUpdateActionFactory Create()
        {
            return new UpdateActionFactory();
        }

        private UpdateActionFactory()
        {
            _creationFuncs = new Dictionary<ContextGameStateActionTypes, Func<JObject, UpdateAction>>();
        }

        void IUpdateActionFactory.RegistrationCreationFunc(ContextGameStateActionTypes actionType, Func<JObject, UpdateAction> creationFunc)
        {
            if (_creationFuncs.ContainsKey(actionType))
            {
                throw new Exception($"UpdateActionFactory.RegistrationActionCreator double registration for type {actionType}");
            }
            
            _creationFuncs.Add(actionType, creationFunc);
        }

        bool IUpdateActionFactory.TryGetActionFromJson(JObject actionObject, out UpdateAction action)
        {
            action = null;
            if (!actionObject.TryGetEnum("action_type", out ContextGameStateActionTypes actionType)
                || !_creationFuncs.TryGetValue(actionType, out var creationFunc))
            {
                return false;
            }
            
            action = creationFunc.Invoke(actionObject);
            return true;
        }

        void IUpdateActionFactory.Cleanup()
        {
            _creationFuncs.Clear();
        }
    }
}