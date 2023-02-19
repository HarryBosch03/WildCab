using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoschingMachine.Kart
{
    public class KartBody : MonoBehaviour
    {
        [SerializeField] private float engineMaxSpeed;
        [SerializeField] private float engineSmoothTime;

        [Space] 
        [FormerlySerializedAs("steerAngle")]
        [SerializeField] private float maxSteerAngle;
        [SerializeField] private float steerSpeed;
        
        [Space]
        [SerializeField] private Transform centerOfMass;

        private new Rigidbody rigidbody;
        private List<KartWheel> wheels;
        private float engineRpm;
        private float engineVelocity;
        private float steerAngle;
        
        public float ThrottlePercent { get; set; }
        public int Steer { get; set; }
        public bool SteerHeld { get; set; }
        public float BrakePercent { get; set; }
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            if (centerOfMass) rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
            
            wheels = new List<KartWheel>(GetComponentsInChildren<KartWheel>());
        }

        private void Update()
        {
            engineRpm = Mathf.SmoothDamp(engineRpm, ThrottlePercent * engineMaxSpeed, ref engineVelocity, engineSmoothTime);

            if (SteerHeld)
            {
                steerAngle += (Steer * maxSteerAngle - steerAngle) * steerSpeed * Time.deltaTime;
                steerAngle = Mathf.Clamp(steerAngle, -maxSteerAngle, maxSteerAngle);
            }
            else
            {
                var turn = Vector3.Dot(transform.up, rigidbody.angularVelocity) * Mathf.Rad2Deg;
                
                var newSteerAngle = steerAngle - turn * Time.deltaTime;
                if (Mathf.Abs(newSteerAngle) < Mathf.Abs(steerAngle)) steerAngle = newSteerAngle;
            }
            
            foreach (var wheel in wheels)
            {
                wheel.Rpm = engineRpm;
                wheel.SteerAngle = steerAngle;
                wheel.BrakePercent = BrakePercent;
            }
        }
    }
}
