using Leopotam.EcsLite;
using Solcery.Models.Components;
using UnityEngine;

namespace Solcery.Models.Systems
{
    sealed class UiTouchSystem : IEcsRunSystem
    {
        public void Run(EcsSystems systems)
        {
            var uiClickComponents = systems.GetWorld().GetPool<UiClickComponent>();
            var uiComponents = systems.GetWorld().GetPool<UiComponent>();
            var filter = systems.GetWorld().Filter<UiComponent>().Inc<UiClickComponent>().End();
            foreach (var entity in filter) 
            {
                // todo reaction
                Debug.Log($"Guid \"{uiComponents.Get(entity).Guid}\" was clicked!");
                uiClickComponents.Del(entity);
            }
        }
    }
}