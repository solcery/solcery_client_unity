using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions.Animation.Factory;
using Solcery.Utils;

namespace Solcery.Games.States.New.Actions.Animation.Visibility
{
    public sealed class UpdateActionAnimationVisibility : UpdateActionAnimation
    {
        public int ObjectId { get; }
        public bool Visible { get; }

        public static UpdateActionAnimation Create(IUpdateActionAnimationFactory factory, int stateId, JObject value)
        {
            return new UpdateActionAnimationVisibility(stateId, value);
        }
        
        // {
        //    "object_id": 2,
        //    "visible": 1,
        //    "animation_id": 19
        // }
        private UpdateActionAnimationVisibility(int stateId, JObject value) : base(stateId)
        {
            ObjectId = value.GetValue<int>("object_id");
            Visible = value.GetValue<int>("visible") == 1;
        }

        public override int GetDurationMsec()
        {
            return 0;
        }
    }
}