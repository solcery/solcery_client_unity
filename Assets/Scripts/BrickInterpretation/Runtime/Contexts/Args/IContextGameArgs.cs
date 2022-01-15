using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.BrickInterpretation.Runtime.Contexts.Args
{
    public interface IContextGameArgs
    {
        Dictionary<string, JObject> Pop();
        void Push(Dictionary<string, JObject> args);
    }
}