using Newtonsoft.Json.Linq;
using Solcery.Utils;
using Solcery.Widgets_new;

namespace Solcery.Games.States.New.Actions.Animation
{
    public sealed class UpdateActionAnimationCardRotate : UpdateAction
    {
        public int ObjectId { get; }
        public int DurationMsec { get; }
        public PlaceWidgetCardFace Face { get; }
        
        public static UpdateAction Create(JObject data)
        {
            return new UpdateActionAnimationCardRotate(data);
        }
        
        // {
        //     "id": 1,
        //     "state_id": 2,
        //     "action_type": 2,
        //     "value": {
        //         "duration": 500,
        //         "object_id": 113,
        //         "face": 0,
        //         "animation_id": 597
        //     }
        // }
        private UpdateActionAnimationCardRotate(JObject data) : base(data)
        {
            var value = data.GetValue<JObject>("value");
            ObjectId = value.GetValue<int>("object_id");
            DurationMsec = value.GetValue<int>("duration");
            Face = value.GetEnum<PlaceWidgetCardFace>("face");
        }
    }
}