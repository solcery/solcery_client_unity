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
        private readonly Dictionary<string, Func<string, Brick>> _brickTypeNameToType;
        private readonly Dictionary<string, Stack<Brick>> _brickPool;

        public static IServiceBricks Create()
        {
            return new ServiceBricks();
        }

        private ServiceBricks()
        {
            _customBricks = new Dictionary<int, JToken>();
            _brickTypeNameToType = new Dictionary<string, Func<string, Brick>>(20);
            _brickPool = new Dictionary<string, Stack<Brick>>(20);
        }

        void IServiceBricks.RegistrationBrickType(string brickTypeName, Func<string, Brick> created, int capacity)
        {
            _brickTypeNameToType.Add(brickTypeName, created);
            _brickPool.Add(brickTypeName, new Stack<Brick>(capacity));
            for (var i = 0; i < capacity; i++)
            {
                _brickPool[brickTypeName].Push(created.Invoke(brickTypeName));
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
            _brickTypeNameToType.Clear();
            _brickPool.Clear();
        }

        private T CreateBrick<T>(string brickTypeName) where T : Brick
        {
            if (!_brickTypeNameToType.ContainsKey(brickTypeName))
            {
                return null;
            }

            var br = _brickPool.ContainsKey(brickTypeName) && _brickPool[brickTypeName].Count > 0
                ? _brickPool[brickTypeName].Pop()
                : _brickTypeNameToType[brickTypeName].Invoke(brickTypeName);

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

            if (!_brickPool.ContainsKey(brick.TypeName))
            {
                _brickPool[brick.TypeName] = new Stack<Brick>(10);
            }
            
            brick.Reset();
            _brickPool[brick.TypeName].Push(brick);
        }

        void IServiceBricks.ExecuteActionBrick(JToken json, IContext context)
        {
            BrickAction brick = null;
            
            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeName(obj, out var brickTypeName))
                {
                    BrickUtils.TryGetBrickParameters(obj, out var parameters);
                    brick = CreateBrick<BrickAction>(brickTypeName);
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
                    && BrickUtils.TryGetBrickTypeName(obj, out var brickTypeName))
                {
                    BrickUtils.TryGetBrickParameters(obj, out var parameters);
                    brick = CreateBrick<BrickValue>(brickTypeName);
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
                    && BrickUtils.TryGetBrickTypeName(obj, out var brickTypeName))
                {
                    BrickUtils.TryGetBrickParameters(obj, out var parameters);
                    brick = CreateBrick<BrickCondition>(brickTypeName);
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