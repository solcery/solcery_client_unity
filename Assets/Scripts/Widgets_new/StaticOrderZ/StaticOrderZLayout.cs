using System.Linq;
using UnityEngine;

namespace Solcery.Widgets_new.StaticOrderZ
{
    public sealed class StaticOrderZLayout : MonoBehaviour
    {
        public static int StaticOrderZCount;
        private static bool _isReordered;
        
        [SerializeField]
        private int orderZ;

        private void Awake()
        {
            StaticOrderZCount++;
        }

        private void Start()
        {
            if (!_isReordered)
            {
                var parent = transform.parent;
                var staticOrderZLayoutList =
                    parent.GetComponentsInChildren<StaticOrderZLayout>().OrderBy(o => o.orderZ).ToList();
                
                foreach (var staticOrderZLayout in staticOrderZLayoutList)
                {
                    transform.SetSiblingIndex(staticOrderZLayoutList.IndexOf(staticOrderZLayout));
                }
                
                _isReordered = true;
            }
        }

        private void OnDestroy()
        {
            StaticOrderZCount--;
            _isReordered = false;
        }
    }
}