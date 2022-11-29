using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.Widgets_new;

namespace Solcery.Games.States.New.Actions.Animation
{
    public sealed class UpdateActionAnimationCardMove : UpdateAction
    {
        public int ObjectId { get; }
        public int DurationMsec { get; }
        public int FromPlaceId { get; }
        public PlaceWidgetCardFace Face { get; }

        public static UpdateAction Create(JObject data)
        {
            return new UpdateActionAnimationCardMove(data);
        }
        
        // {
        //     "id": 1,
        //     "state_id": 0,
        //     "action_type": 2,
        //     "value": {
        //         "duration": 1000,
        //         "from_place_id": 3,
        //         "object_id": 103,
        //         "face": 0,
        //         "animation_id": 596
        //     }
        // }
        private UpdateActionAnimationCardMove(JObject data) : base(data)
        {
            var value = data.GetValue<JObject>("value");
            ObjectId = value.GetValue<int>("object_id");
            DurationMsec = value.GetValue<int>("duration");
            FromPlaceId = value.GetValue<int>("from_place_id");
            Face = value.GetEnum<PlaceWidgetCardFace>("face");
        }
    }
}