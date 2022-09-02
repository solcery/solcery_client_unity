using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;

namespace Solcery.Models.Play.Places
{
    public struct ComponentPlaceAvailableBrick : IEcsAutoReset<ComponentPlaceAvailableBrick>
    {
        public bool HasAvailableBrick => AvailableBrick != null;
        public JObject AvailableBrick;

        public void AutoReset(ref ComponentPlaceAvailableBrick c)
        {
            c.AvailableBrick = null;
        }
    }
}