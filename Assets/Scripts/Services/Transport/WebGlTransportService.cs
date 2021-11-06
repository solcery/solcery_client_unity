using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.Transport
{
    public sealed class WebGlTransportService : ITransportService
    {
        private readonly List<Action<JObject>> _listEventReceivingGameContent;
        private readonly List<Action<JObject>> _listEventReceivingGameState;

        event Action<JObject> ITransportService.EventReceivingGameContent
        {
            add
            {
                if (!_listEventReceivingGameContent.Contains(value))
                {
                    _listEventReceivingGameContent.Add(value);
                }
            }
            
            remove => _listEventReceivingGameContent.Remove(value);
        }

        event Action<JObject> ITransportService.EventReceivingGameState
        {
            add
            {
                if (!_listEventReceivingGameState.Contains(value))
                {
                    _listEventReceivingGameState.Add(value);
                }
            }

            remove => _listEventReceivingGameState.Remove(value);
        }

        public static ITransportService Create()
        {
            return new WebGlTransportService();
        }

        private WebGlTransportService()
        {
            _listEventReceivingGameContent = new List<Action<JObject>>();
            _listEventReceivingGameState = new List<Action<JObject>>();
        }
        
        void ITransportService.CallUnityLoaded()
        {
            throw new NotImplementedException();
        }

        void ITransportService.SendCommand(JObject command)
        {
            throw new NotImplementedException();
        }
        
        void ITransportService.Cleanup()
        {
            throw new NotImplementedException();
        }
        
        void ITransportService.Destroy()
        {
            throw new NotImplementedException();
        }
    }
}