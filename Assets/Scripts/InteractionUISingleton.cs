using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class InteractionUISingleton : MonoBehaviour
    {
        private static GameObject instance;

        private InteractionUISingleton()
        {
            // Private constructor to prevent instantiation from outside the class.
        }

        public void Start()
        {
            this.gameObject.SetActive(false);
            instance = this.gameObject;
        }

        public static GameObject Instance
        {
            get
            {
                return instance;
            }
        }


    }
}
