using Leopotam.EcsLite;

namespace Solcery.Models.Shared.Objects
{
    public struct ComponentObjectIdHash : IEcsAutoReset<ComponentObjectIdHash>
    {
        public IObjectIdHash ObjectIdHashes;


        public void AutoReset(ref ComponentObjectIdHash c)
        {
            c.ObjectIdHashes ??= ObjectIdHash.Create();
            c.ObjectIdHashes.Reset();
        }
    }
}