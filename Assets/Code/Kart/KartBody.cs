using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Kart
{
    public class KartBody : MonoBehaviour
    {
        [SerializeField] private float engineMaxSpeed;
        [SerializeField] private float engineSmoothTime;

        [Space] 
        [SerializeField] private float steerAngle;

        private List<KartWheel> wheels;
        private float engineRpm;
        private float engineVelocity;
        
        public float ThrottlePercent { get; set; }
        public float SteerPercent { get; set; }
        public float BrakePercent { get; set; }
        
        private void Awake()
        {
            wheels = new List<KartWheel>(GetComponentsInChildren<KartWheel>());
        }

        private void Update()
        {
            engineRpm = Mathf.SmoothDamp(engineRpm, ThrottlePercent * engineMaxSpeed, ref engineVelocity, engineSmoothTime);
            
            foreach (var wheel in wheels)
            {
                wheel.Rpm = engineRpm;
                wheel.SteerAngle = SteerPercent * steerAngle;
                wheel.BrakePercent = BrakePercent;
            }
        }
    }
}
