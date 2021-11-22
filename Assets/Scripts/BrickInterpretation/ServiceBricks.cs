using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
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

        void IServiceBricks.RegistrationCustomBricksData(JArray customBricksJson)
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

        private bool IsCustomBrick(int subtype)
        {
            return _customBricks.ContainsKey(subtype);
        }

        void IServiceBricks.Cleanup()
        {
            Cleanup();
        }
        
        void IServiceBricks.Destroy()
        {
            Cleanup();
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

        void IServiceBricks.ExecuteActionBrick(JToken json, IContext context)
        {
            BrickAction brick = null;
            
            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType))
                {
                    BrickUtils.TryGetBrickParameters(obj, out var parameters);
                    brick = CreateBrick<BrickAction>(typeSubType.Item1, typeSubType.Item2);
                    brick.Run(this, parameters, context);
                }
            }
            finally
            {
                FreeBrick(brick);
            }
        }

        int IServiceBricks.ExecuteValueBrick(JToken json, IContext context)
        {
            var result = 0;
            BrickValue brick = null;

            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType))
                {
                    BrickUtils.TryGetBrickParameters(obj, out var parameters);
                    brick = CreateBrick<BrickValue>(typeSubType.Item1, typeSubType.Item2);
                    result = brick.Run(this, parameters, context);
                }
            }
            finally
            {
                FreeBrick(brick);
            }

            return result;
        }

        bool IServiceBricks.ExecuteConditionBrick(JToken json, IContext context)
        {
            var result = false;
            BrickCondition brick = null;
            
            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeSubType(obj, out var typeSubType))
                {
                    BrickUtils.TryGetBrickParameters(obj, out var parameters);
                    brick = CreateBrick<BrickCondition>(typeSubType.Item1, typeSubType.Item2);
                    result = brick.Run(this, parameters, context);
                }
            }
            finally
            {
                FreeBrick(brick);
            }

            return result;
        }
    }
}