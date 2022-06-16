using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Solcery

{
    //[ExecuteAlways]
    public class MatFloater : MonoBehaviour

    {
        public string property;
        public float value = 1;

        //private Image rend;
        private Renderer rend;
        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<Renderer>();
        }

        // Update is called once per frame
        void Update()
        {
            rend.material.SetFloat(property,value);
        }
    }
}
