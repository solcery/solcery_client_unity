using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions.Animation.Factory;
using Solcery.Utils;
using Solcery.Widgets_new;

namespace Solcery.Games.States.New.Actions.Animation.Move
{
    public sealed class UpdateActionAnimationMove : UpdateActionAnimation
    {
        public int ObjectId { get; }
        public int DurationMsec { get; }
        public int FromPlaceId { get; }
        public PlaceWidgetCardFace Face { get; }

        public static UpdateActionAnimation Create(IUpdateActionAnimationFactory factory, int stateId, JObject value)
        {
            return new UpdateActionAnimationMove(stateId, value);
        }
        
        // {
        //    "duration": 1000,
        //    "from_place_id": 3,
        //    "object_id": 103,
        //    "face": 0,
        //    "animation_id": 596
        // }
        private UpdateActionAnimationMove(int stateId, JObject value) : base(stateId)
        {
            ObjectId = value.GetValue<int>("object_id");
            DurationMsec = value.GetValue<int>("duration");
            FromPlaceId = value.GetValue<int>("from_place_id");
            Face = value.GetEnum<PlaceWidgetCardFace>("face");
        }

        public override int GetDurationMsec()
        {
            return DurationMsec;
        }
    }
}