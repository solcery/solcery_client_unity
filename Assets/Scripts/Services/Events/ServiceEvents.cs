using System;
using System.Collections.Generic;

namespace Solcery.Services.Events
{
    public sealed class ServiceEvents : IServiceEvents
    {
        private static readonly Lazy<ServiceEvents> Lazy = new(() => new ServiceEvents());

        public static IServiceEvents Current => Lazy.Value;

        private readonly Dictionary<string, HashSet<IEventListener>> _eventListeners;

        private ServiceEvents()
        {
            _eventListeners = new Dictionary<string, HashSet<IEventListener>>();
        }

        IServiceEvents IServiceEvents.AddListener(string eventKey, IEventListener eventListener)
        {
            if (!_eventListeners.ContainsKey(eventKey))
            {
                _eventListeners.Add(eventKey, new HashSet<IEventListener>());
            }

            _eventListeners[eventKey].Add(eventListener);

            return this;
        }

        IServiceEvents IServiceEvents.RemoveListener(string eventKey, IEventListener eventListener)
        {
            if (_eventListeners.ContainsKey(eventKey))
            {
                _eventListeners[eventKey].Remove(eventListener);
            }

            return this;
        }

        void IServiceEvents.BroadcastEvent(EventData eventData)
        {
            if (_eventListeners.TryGetValue(eventData.EventName, out var eventListeners))
            {
                foreach (var eventListener in eventListeners)
                {
                    eventListener.OnEvent(eventData);
                }
            }
        }

        void IServiceEvents.Cleanup()
        {
            _eventListeners.Clear();
        }
    }
}