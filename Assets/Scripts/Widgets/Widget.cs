using Leopotam.EcsLite;

namespace Solcery.Widgets
{
    public abstract class Widget
    {
        public abstract void UpdateWidget(EcsWorld world, int entityId);
    }
}