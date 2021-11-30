using System;
using Newtonsoft.Json.Linq;
using Solcery.Services.Commands;

namespace Solcery.Services.Transport
{
    public sealed class WebGlTransportService : ITransportService
    {
        public static ITransportService Create()
        {
            return new WebGlTransportService();
        }

        private WebGlTransportService()
        {
        }
        
        void ITransportService.CallUnityLoaded()
        {
            throw new NotImplementedException();
        }

        void ITransportService.SendCommand(JObject command)
        {
            throw new NotImplementedException();
        }

        void ITransportService.Update(float dt)
        {
            
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