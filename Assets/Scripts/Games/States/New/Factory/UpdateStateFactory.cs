using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.Contexts.GameStates;
using Solcery.Games.States.New.States;
using Solcery.Utils;

namespace Solcery.Games.States.New.Factory
{
    public sealed class UpdateStateFactory : IUpdateStateFactory
    {
        private bool _isInit;
        
        private readonly Dictionary<ContextGameStateTypes, Func<ContextGameStateTypes, UpdateState>> _creators;
        private readonly Dictionary<ContextGameStateTypes, Stack<UpdateState>> _pool;

        public static IUpdateStateFactory Create()
        {
            return new UpdateStateFactory();
        }

        private UpdateStateFactory()
        {
            _isInit = false;
            _creators = new Dictionary<ContextGameStateTypes, Func<ContextGameStateTypes, UpdateState>>();
            _pool = new Dictionary<ContextGameStateTypes, Stack<UpdateState>>();
        }

        void IUpdateStateFactory.RegistrationCreationFunc(ContextGameStateTypes updateStateType, Func<ContextGameStateTypes, UpdateState> updateStateCreationFunc)
        {
            if (_creators.ContainsKey(updateStateType))
            {
                throw new Exception($"You are trying to register a duplicate type {updateStateType}!");
            }
            
            _creators.Add(updateStateType, updateStateCreationFunc);
        }

        void IUpdateStateFactory.Init()
        {
            if (_isInit)
            {
                return;
            }

            _pool.Clear();
            foreach (var (updateStateType, updateStateCreationFunc) in _creators)
            {
                _pool.Add(updateStateType, new Stack<UpdateState>());
                for (var i = 0; i < 10; i++)
                {
                    _pool[updateStateType].Push(updateStateCreationFunc.Invoke(updateStateType));
                }
            }
            
            _isInit = true;
        }

        UpdateState IUpdateStateFactory.ConstructFromJObject(JObject stateUpdateObject, bool isPredictable)
        {
            if (!_isInit)
            {
                throw new Exception("You need to call UpdateStateFactory.Init before using this function.");
            }

            var type = stateUpdateObject.GetEnum<ContextGameStateTypes>("state_type");

            if (!_creators.ContainsKey(type))
            {
                throw new Exception($"You are trying create undefined UpdateStateType => {type}.");
            }

            UpdateState result;

            if (_pool.ContainsKey(type) && _pool[type].Count > 0)
            {
                result = _pool[type].Pop();
            }
            else
            {
                result = _creators[type].Invoke(type);
            }

            result.Init(stateUpdateObject.GetValue<JObject>("value"), isPredictable);
            return result;
        }

        void IUpdateStateFactory.Deconstruct(UpdateState updateState)
        {
            if (!_isInit)
            {
                throw new Exception("You need to call UpdateStateFactory.Init before using this function.");
            }
            
            updateState.Cleanup();

            if (!_pool.ContainsKey(updateState.UpdateStateType))
            {
                _pool.Add(updateState.UpdateStateType, new Stack<UpdateState>());
            }
            
            _pool[updateState.UpdateStateType].Push(updateState);
        }
    }
}