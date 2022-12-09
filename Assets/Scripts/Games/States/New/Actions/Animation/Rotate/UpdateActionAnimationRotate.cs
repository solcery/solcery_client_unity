using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions.Animation.Factory;
using Solcery.Utils;
using Solcery.Widgets_new;

namespace Solcery.Games.States.New.Actions.Animation.Rotate
{
    public sealed class UpdateActionAnimationRotate : UpdateActionAnimation
    {
        public int ObjectId { get; }
        public int DurationMsec { get; }
        public PlaceWidgetCardFace Face { get; }
        
        public static UpdateActionAnimation Create(IUpdateActionAnimationFactory factory, int stateId, JObject value)
        {
            return new UpdateActionAnimationRotate(stateId, value);
        }
        
        // {
        //    "duration": 500,
        //    "object_id": 113,
        //    "face": 0,
        //    "animation_id": 597
        // }
        private UpdateActionAnimationRotate(int stateId, JObject value) : base(stateId)
        {
            ObjectId = value.GetValue<int>("object_id");
            DurationMsec = value.GetValue<int>("duration");
            Face = value.GetEnum<PlaceWidgetCardFace>("face");
        }

        public override int GetDurationMsec()
        {
            return DurationMsec;
        }
    }
}