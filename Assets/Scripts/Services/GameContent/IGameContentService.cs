using System;
using Newtonsoft.Json.Linq;

namespace Solcery.Services.GameContent
{
    public interface IGameContentService
    {
        public event Action EventOnReceivingGameContent;
        public event Action<JObject> EventOnReceivingUi;
        public event Action<JObject> EventOnReceivingGame;

        public void Init();
        public void Update();
        public void Cleanup();
        public void Destroy();
    }
}