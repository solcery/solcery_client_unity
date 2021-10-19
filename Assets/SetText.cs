using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Solcery
{
    public class SetText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        
        // Start is called before the first frame update
        void Start()
        {
            text.text = "Prod";
#if DEV
            text.text = "Dev";
#endif
        }
    }
}
