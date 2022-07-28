using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectType : IEcsAutoReset<ComponentObjectType>
    {
        public int TplId;

        public void AutoReset(ref ComponentObjectType c)
        {
            c.TplId = -1;
        }
    }
}