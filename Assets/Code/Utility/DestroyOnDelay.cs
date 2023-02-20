using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public class DestroyOnDelay : MonoBehaviour
    {
        [SerializeField] float delay;

        private void Start()
        {
            Destroy(gameObject, delay);
        }
    }
}
