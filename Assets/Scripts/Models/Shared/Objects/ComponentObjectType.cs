using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectType : IEcsAutoReset<ComponentObjectType>
    {
        public int Type;

        public void AutoReset(ref ComponentObjectType c)
        {
            c.Type = -1;
        }
    }
}