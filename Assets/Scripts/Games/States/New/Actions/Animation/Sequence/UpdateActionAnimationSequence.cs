using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions.Animation.Factory;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.Animation.Sequence
{
    public sealed class UpdateActionAnimationSequence : UpdateActionAnimation
    {
        public IReadOnlyList<UpdateActionAnimation> Actions => _actions;

        private readonly List<UpdateActionAnimation> _actions;
        private readonly int _durationMsec;

        public static UpdateActionAnimation Create(IUpdateActionAnimationFactory factory, int stateId, JObject value)
        {
            return new UpdateActionAnimationSequence(factory, stateId, value);
        }
        
        // {
        //    "animation_id": 15,
        //    "sequence": 
        //    [
        //       {
        //          // animation_1
        //       },
        //       {
        //          // animation_2
        //       },
        //       {
        //          // animation_n-1
        //       },
        //       {
        //          // animation_n
        //       }
        //    ]
        // }
        private UpdateActionAnimationSequence(IUpdateActionAnimationFactory factory, int stateId, JObject value) : base(stateId)
        {
            _actions = new List<UpdateActionAnimation>();
            _durationMsec = 0;
            
            if (value.TryGetValue("sequence", out JArray sequences))
            {
                foreach (var animationValueToken in sequences)
                {
                    if (animationValueToken is JObject animationValue)
                    {
                        var updateAction = factory.CreateAction(stateId, animationValue);
                        updateAction.UpdateDelay(_durationMsec);
                        _durationMsec += updateAction.GetDurationMsec();
                        _actions.Add(updateAction);
                    }
                }
            }
        }

        public override int GetDurationMsec()
        {
            return _durationMsec;
        }
    }
}