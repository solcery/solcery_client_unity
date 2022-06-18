using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceVisibilityBrick : IEcsAutoReset<ComponentPlaceVisibilityBrick>
    {
        public bool HasVisibilityBrick => VisibilityBrick != null;
        public JObject VisibilityBrick;

        public void AutoReset(ref ComponentPlaceVisibilityBrick c)
        {
            c.VisibilityBrick = null;
        }
    }
}