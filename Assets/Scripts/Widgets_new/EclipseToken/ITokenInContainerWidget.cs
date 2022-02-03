using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.Widgets_new.EclipseToken
{
    public interface ITokenInContainerWidget
    {
        void IncreaseCounter();
        void UpdateParent(Transform parent);
        void UpdateFromCardTypeData(int objectId, JObject data);
        void Cleanup();
        void Destroy();
    }
}
