using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation
{
    public class ServiceBricks : IServiceBricks
    {
        private readonly Dictionary<int, JToken> _customBricks;

        private readonly Dictionary<int, Dictionary<int, Func<int, int, Brick>>> _creationFuncForTypesSubtypes;
        private readonly Dictionary<int, Dictionary<int, Stack<Brick>>> _poolOfBricks;


        public static IServiceBricks Create()
        {
            return new ServiceBricks();
        }

        private ServiceBricks()
        {
            _customBricks = new Dictionary<int, JToken>();
            _creationFuncForTypesSubtypes = new Dictionary<int, Dictionary<int, Func<int, int, Brick>>>(3);
            _poolOfBricks = new Dictionary<int, Dictionary<int, Stack<Brick>>>(3);
        }

        #region IServiceBricks implementation
        
        void IServiceBricks.RegistrationCustomBricksData(JArray customBricksJson)
        {
            RegistrationCustomBricksData(customBricksJson);
        }
        
        void IServiceBricks.RegistrationBrickType(BrickTypes type, BrickActionTypes subType, Func<int, int, Brick> created, uint capacity)
        {
            RegistrationBrickType((int)type, (int)subType, created, capacity);
        }
        
        void IServiceBricks.RegistrationBrickType(BrickTypes type, BrickConditionTypes subType, Func<int, int, Brick> created, uint capacity)
        {
            RegistrationBrickType((int)type, (int)subType, created, capacity);
        }

        void IServiceBricks.RegistrationBrickType(BrickTypes type, BrickValueTypes subType, Func<int, int, Brick> created, uint capacity)
        {
            RegistrationBrickType((int)type, (int)subType, created, capacity);
        }
        
        void IServiceBricks.Cleanup()
        {
            Cleanup();
        }
        
        void IServiceBricks.Destroy()
        {
            Cleanup();
        }
        
        bool IServiceBricks.ExecuteActionBrick(JToken json, EcsWorld world)
        {
            return ExecuteActionBrick(json, world);
        }
        
        bool IServiceBricks.ExecuteValueBrick(JToken json, EcsWorld world, out int result)
        {
            return ExecuteValueBrick(json, world, out result);
        }
        
        bool IServiceBricks.ExecuteConditionBrick(JToken json, EcsWorld world, out bool result)
        {
            return ExecuteConditionBrick(json, world, out result);
        }
        
        #endregion

        #region Private method implementation
        
        private void RegistrationCustomBricksData(JArray customBricksJson)
        {
            _customBricks.Clear();

            foreach (var customBrickToken in customBricksJson)
            {
                if(customBrickToken is JObject customBrickObject)
                {
                    var id = 10000 + customBrickObject.GetValue<int>("id");
                    _customBricks.Add(id, customBrickObject.GetValue<JObject>("brick"));
                }
            }
        }
        
        private void RegistrationBrickType(int type, int subType, Func<int, int, Brick> created, uint capacity)
        {
            if (!_creationFuncForTypesSubtypes.ContainsKey(type))
            {
                _creationFuncForTypesSubtypes.Add(type, new Dictionary<int, Func<int, int, Brick>>(20));
            }
            
            var brickPoolCount = capacity >= int.MaxValue ? int.MaxValue : (int)capacity;
            _creationFuncForTypesSubtypes[type].Add(subType, created);

            if (!_poolOfBricks.ContainsKey(type))
            {
                _poolOfBricks.Add(type, new Dictionary<int, Stack<Brick>>(brickPoolCount));
            }

            if (!_poolOfBricks[type].ContainsKey(subType))
            {
                _poolOfBricks[type].Add(subType, new Stack<Brick>(20));
            }

            for (var i = 0; i < brickPoolCount; i++)
            {
                _poolOfBricks[type][subType].Push(created.Invoke(type, subType));
            }
        }
        
        private bool IsCustomBrick(int subtype)
        {
            return _customBricks.ContainsKey(subtype);
        }
        
        private void Cleanup()
        {
            _customBricks.Clear();
            _creationFuncForTypesSubtypes.Clear();
            _poolOfBricks.Clear();
        }

        private T CreateBrick<T>(int type, int subType) where T : Brick
        {
            if (!_creationFuncForTypesSubtypes.ContainsKey(type) 
                || !_creationFuncForTypesSubtypes[type].ContainsKey(subType))
            {
                return null;
            }

            var br = _poolOfBricks.TryGetValue(type, out var brickTypeMap)
                     && brickTypeMap.TryGetValue(subType, out var brickSubtypeStack)
                     && brickSubtypeStack.Count > 0
                ? brickSubtypeStack.Pop()
                : _creationFuncForTypesSubtypes[type][subType].Invoke(type, subType);

            if (br is T brT)
            {
                return brT;
            }
            
            FreeBrick(br);
            return null;
        }

        private void FreeBrick(Brick brick)
        {
            if (brick == null)
            {
                return;
            }

            if (!_poolOfBricks.ContainsKey(brick.Type))
            {
                _poolOfBricks.Add(brick.Type, new Dictionary<int, Stack<Brick>>());
            }

            if (!_poolOfBricks[brick.Type].ContainsKey(brick.SubType))
            {
                _poolOfBricks[brick.Type].Add(brick.SubType, new Stack<Brick>(20));
            }
            
            brick.Reset();
            _poolOfBricks[brick.Type][brick.SubType].Push(brick);
        }

        private Dictionary<string, JToken> CreateCustomArgs(JArray customParameters)
        {
            var arg = new Dictionary<string, JToken>(customParameters.Count);
            
            foreach (var customParameterToken in customParameters)
            {
                if (customParameterToken is JObject customParameterObject 
                    && customParameterObject.TryGetValue("name", out string name)
                    && customParameterObject.TryGetValue("value", out JObject brick))
                {
                    arg.Add(name, brick);
                }
            }

            return arg;
        }

        private bool ExecuteActionCustomBrick(JToken json, EcsWorld world)
        {
            var completed = false;
            var filter = world.Filter<ComponentContextArgs>().End();
            
            if (json is JObject obj
                && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType)
                && BrickUtils.TryGetBrickParameters(obj, out var customParameters)
                && _customBricks.TryGetValue(typeSubType.Item2, out var customBrickToken)
                && filter.GetEntitiesCount() > 0)
            {
                foreach (var entityId in filter)
                {
                    ref var args = ref world.GetPool<ComponentContextArgs>().Get(entityId);
                    args.Push(CreateCustomArgs(customParameters));
                    completed = ExecuteActionBrick(customBrickToken, world);
                    args.Pop();
                    break;
                }
            }

            return completed;
        }

        private bool ExecuteValueCustomBrick(JToken json, EcsWorld world, out int result)
        {
            result = 0;
            var completed = false;
            var filter = world.Filter<ComponentContextArgs>().End();
            
            if (json is JObject obj
                && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType)
                && BrickUtils.TryGetBrickParameters(obj, out var customParameters)
                && _customBricks.TryGetValue(typeSubType.Item2, out var customBrickToken)
                && filter.GetEntitiesCount() > 0)
            {
                foreach (var entityId in filter)
                {
                    ref var args = ref world.GetPool<ComponentContextArgs>().Get(entityId);
                    args.Push(CreateCustomArgs(customParameters));
                    completed = ExecuteValueBrick(customBrickToken, world, out result);
                    args.Pop();
                    break;
                }
            }

            return completed;
        }

        private bool ExecuteConditionCustomBrick(JToken json, EcsWorld world, out bool result)
        {
            result = false;
            var completed = false;
            var filter = world.Filter<ComponentContextArgs>().End();
            
            if (json is JObject obj
                && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType)
                && BrickUtils.TryGetBrickParameters(obj, out var customParameters)
                && _customBricks.TryGetValue(typeSubType.Item2, out var customBrickToken)
                && filter.GetEntitiesCount() > 0)
            {
                foreach (var entityId in filter)
                {
                    ref var args = ref world.GetPool<ComponentContextArgs>().Get(entityId);
                    args.Push(CreateCustomArgs(customParameters));
                    completed = ExecuteConditionBrick(customBrickToken, world, out result);
                    args.Pop();
                    break;
                }
            }

            return completed;
        }

        private bool ExecuteActionBrick(JToken json, EcsWorld world)
        {
            var completed = false;
            BrickAction brick = null;

            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType)
                    && !IsCustomBrick(typeSubType.Item2)
                    && BrickUtils.TryGetBrickParameters(obj, out var parameters))
                {
                    brick = CreateBrick<BrickAction>(typeSubType.Item1, typeSubType.Item2);
                    brick.Run(this, parameters, world);
                    completed = true;
                }
            }
            finally
            {
                FreeBrick(brick);
            }

            return completed || ExecuteActionCustomBrick(json, world);
        }

        private bool ExecuteValueBrick(JToken json, EcsWorld world, out int result)
        {
            result = 0;
            var completed = false;
            BrickValue brick = null;

            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType)
                    && !IsCustomBrick(typeSubType.Item2)
                    && BrickUtils.TryGetBrickParameters(obj, out var parameters))
                {
                    brick = CreateBrick<BrickValue>(typeSubType.Item1, typeSubType.Item2);
                    result = brick.Run(this, parameters, world);
                    completed = true;
                }
            }
            finally
            {
                FreeBrick(brick);
            }

            return completed || ExecuteValueCustomBrick(json, world, out result);
        }

        private bool ExecuteConditionBrick(JToken json, EcsWorld world, out bool result)
        {
            result = false;
            var completed = false;
            BrickCondition brick = null;
            
            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType)
                    && !IsCustomBrick(typeSubType.Item2)
                    && BrickUtils.TryGetBrickParameters(obj, out var parameters))
                {
                    brick = CreateBrick<BrickCondition>(typeSubType.Item1, typeSubType.Item2);
                    result = brick.Run(this, parameters, world);
                    completed = true;
                }
            }
            finally
            {
                FreeBrick(brick);
            }

            return completed || ExecuteConditionCustomBrick(json, world, out result);
        }

        #endregion
    }
}