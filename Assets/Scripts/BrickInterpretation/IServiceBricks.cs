using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;

namespace Solcery.BrickInterpretation
{
    public interface IServiceBricks
    {
        void RegistrationBrickType(BrickTypes type, BrickActionTypes subType, Func<int, int, Brick> created, uint capacity = 1);
        void RegistrationBrickType(BrickTypes type, BrickConditionTypes subType, Func<int, int, Brick> created, uint capacity = 1);
        void RegistrationBrickType(BrickTypes type, BrickValueTypes subType, Func<int, int, Brick> created, uint capacity = 1);
        void RegistrationCustomBricksData(JArray customBricksJson);
        bool ExecuteActionBrick(JObject brickObject, EcsWorld world, int level);
        bool ExecuteValueBrick(JObject brickObject, EcsWorld world, int level, out int result);
        bool ExecuteConditionBrick(JObject brickObject, EcsWorld world, int level, out bool result);
        void Destroy();
    }
}