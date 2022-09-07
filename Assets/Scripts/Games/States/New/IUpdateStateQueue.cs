using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.States;

namespace Solcery.Games.States.New
{
    public interface IUpdateStateQueue
    {
        bool IsPredictable { get; }
        UpdateState CurrentState { get; }
        void PushGameState(JObject gameState);
        void Update(int deltaTimeMsec);
        bool TryGetActionForStateId(int stateId, out List<UpdateAction> actions);
        void RemoveAllActionForStateId(int stateId);
    }
}