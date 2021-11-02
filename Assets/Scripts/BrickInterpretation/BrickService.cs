using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Utils;

namespace Solcery.BrickInterpretation
{
    public class BrickService : IBrickService
    {
        private readonly Dictionary<string, Func<string, Brick>> _brickTypeNameToType;
        private readonly Dictionary<string, Stack<Brick>> _brickPool;

        public static IBrickService Create()
        {
            return new BrickService();
        }

        private BrickService()
        {
            _brickTypeNameToType = new Dictionary<string, Func<string, Brick>>(20);
            _brickPool = new Dictionary<string, Stack<Brick>>(20);
        }

        void IBrickService.RegistrationBrickType(string brickTypeName, Func<string, Brick> created, int capacity = 1)
        {
            _brickTypeNameToType.Add(brickTypeName, created);
            _brickPool.Add(brickTypeName, new Stack<Brick>(capacity));
            for (var i = 0; i < capacity; i++)
            {
                _brickPool[brickTypeName].Push(created.Invoke(brickTypeName));
            }
        }

        void IBrickService.Cleanup()
        {
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

        void IBrickService.ExecuteActionBrick(JToken json, IContext context)
        {
            BrickAction brick = null;
            
            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeName(obj, out var brickTypeName)
                    && BrickUtils.TryGetBrickParameters(obj, out var parameters))
                {
                    brick = CreateBrick<BrickAction>(brickTypeName);
                    brick.Run(this, parameters, context);
                }
            }
            finally
            {
                FreeBrick(brick);
            }
        }

        int IBrickService.ExecuteValueBrick(JToken json, IContext context)
        {
            var result = 0;
            BrickValue brick = null;

            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeName(obj, out var brickTypeName)
                    && BrickUtils.TryGetBrickParameters(obj, out var parameters))
                {
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

        bool IBrickService.ExecuteConditionBrick(JToken json, IContext context)
        {
            var result = false;
            BrickCondition brick = null;
            
            try
            {
                if (json is JObject obj 
                    && BrickUtils.TryGetBrickTypeName(obj, out var brickTypeName)
                    && BrickUtils.TryGetBrickParameters(obj, out var parameters))
                {
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