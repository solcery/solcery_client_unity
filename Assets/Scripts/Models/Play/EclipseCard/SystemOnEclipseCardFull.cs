using Leopotam.EcsLite;
using Solcery.Services.Events;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Cards.EventsData;
using UnityEngine;

namespace Solcery.Models.Play.EclipseCard
{
    public interface ISystemOnEclipseCardFull : IEventListener, IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
    }
    
    public class SystemOnEclipseCardFull : ISystemOnEclipseCardFull
    {
        private EventData _uiEventData;
        
        public static SystemOnEclipseCardFull Create()
        {
            return new SystemOnEclipseCardFull();
        }
        
        private SystemOnEclipseCardFull() { }     
        
        void IEcsInitSystem.Init(EcsSystems systems)
        {
            ServiceEvents.Current.AddListener(OnEclipseCardFullEventData.OnEclipseCardFullEventName, this);
        }
        
        void IEcsDestroySystem.Destroy(EcsSystems systems)
        {
            ServiceEvents.Current.RemoveListener(OnEclipseCardFullEventData.OnEclipseCardFullEventName, this);
        }

        void IEcsRunSystem.Run(EcsSystems systems)
        {
            if (_uiEventData == null)
            {
                return;
            }
            
            if (_uiEventData is OnEclipseCardFullEventData cardFullEventData)
            {
                OnEclipseCardFull(cardFullEventData.EclipseCard);
            }
            
            _uiEventData = null;
        }

        void IEventListener.OnEvent(EventData eventData)
        {
            if (eventData.EventName == OnEclipseCardFullEventData.OnEclipseCardFullEventName)
            {
                _uiEventData = eventData;
            }
        }
        
        private void OnEclipseCardFull(EclipseCardInContainerWidgetLayout eclipseCard)
        {
            if (eclipseCard != null)
            {
                GameApplication.Game().FullModeController.Show(eclipseCard);
            }
            else
            {
                Debug.LogWarning("Pattern for full mode is empty!");
            }
        }        
    }
}