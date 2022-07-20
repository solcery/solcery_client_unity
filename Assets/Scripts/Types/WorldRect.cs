using UnityEngine;

namespace Solcery.Types
{
    public class WorldRect
    {
        public Vector3 BottomLeft => _corners[0];
        public Vector3 TopLeft => _corners[1];
        public Vector3 TopRight => _corners[2];
        public Vector3 BottomRight => _corners[3];

        public float xMin => BottomLeft.x;
        public float xMax => BottomRight.x;
        public float yMin => BottomLeft.y;
        public float yMax => TopLeft.y;
        
        private readonly Vector3[] _corners;

        public static WorldRect Create(RectTransform rectTransform)
        {
            return new WorldRect(rectTransform);
        }
        
        private WorldRect(RectTransform rectTransform)
        {
            _corners = new Vector3[4];
            rectTransform.GetWorldCorners(_corners);
        }
    }
}