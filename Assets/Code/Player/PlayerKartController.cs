using System;
using System.Collections;
using System.Collections.Generic;
using BoschingMachine.Kart;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoschingMachine
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(KartBody))]
    public class PlayerKartController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputAsset;
        
        private KartBody kart;

        private InputActionMap kartMap;
        private InputAction throttle;
        private InputAction steer;
        private InputAction brake;

        private void Awake()
        {
            kart = GetComponent<KartBody>();

            kartMap = inputAsset.FindActionMap("kart");
            kartMap.Enable();
            
            throttle = kartMap.FindAction("throttle");
            steer = kartMap.FindAction("steer");
            brake = kartMap.FindAction("brake");
        }

        private void Update()
        {
            kart.ThrottlePercent = throttle.ReadValue<float>();
            kart.SteerPercent = steer.ReadValue<float>();
            kart.BrakePercent = brake.ReadValue<float>();
        }
    }
}
