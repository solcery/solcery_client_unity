using Leopotam.EcsLite;

namespace Solcery.Models.Simulation.Game.State
{
    public struct ComponentGameStateIndex : IEcsAutoReset<ComponentGameStateIndex>
    {
        public int Index;
        
        public void AutoReset(ref ComponentGameStateIndex c)
        {
            c.Index = 0;
        }
    }
}