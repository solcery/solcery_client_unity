using Newtonsoft.Json.Linq;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Services.Widget.Data
{
    public class TransformData
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;
        
        public static TransformData Parse(JObject obj)
        {
            return new TransformData(obj);
        }

        private TransformData(JObject obj)
        {
            if (obj.ContainsKey("position"))
            {
                Position = obj.GetVector3("position");
            }
            if (obj.ContainsKey("rotation"))
            {
                Position = obj.GetVector3("rotation");
            }
            if (obj.ContainsKey("scale"))
            {
                Position = obj.GetVector3("scale");
            }
        }
    }
}