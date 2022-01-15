using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Actions;
using Solcery.BrickInterpretation.Runtime.Conditions;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Values;

namespace Solcery.BrickInterpretation.Runtime
{
    public interface IServiceBricks
    {
        void RegistrationBrickType(BrickTypes type, BrickActionTypes subType, Func<int, int, Brick> created, uint capacity = 1);
        void RegistrationBrickType(BrickTypes type, BrickConditionTypes subType, Func<int, int, Brick> created, uint capacity = 1);
        void RegistrationBrickType(BrickTypes type, BrickValueTypes subType, Func<int, int, Brick> created, uint capacity = 1);
        void RegistrationCustomBricksData(JArray customBricksJson);
        bool ExecuteActionBrick(JObject brickObject, IContext context, int level);
        bool ExecuteValueBrick(JObject brickObject, IContext context, int level, out int result);
        bool ExecuteConditionBrick(JObject brickObject, IContext context, int level, out bool result);
        void Destroy();
    }
}