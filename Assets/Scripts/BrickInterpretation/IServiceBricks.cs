using System;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation
{
    public interface IServiceBricks
    {
        void RegistrationBrickType(string brickTypeName, Func<string, Brick> created, int capacity = 1);
        void RegistrationCustomBricksData(JArray customBricksJson);
        void ExecuteActionBrick(JToken json, IContext context);
        int ExecuteValueBrick(JToken json, IContext context);
        bool ExecuteConditionBrick(JToken json, IContext context);
        void Cleanup();
        void Destroy();
    }
}