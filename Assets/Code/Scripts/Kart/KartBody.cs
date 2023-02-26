using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoschingMachine.Kart
{
    public class KartBody : MonoBehaviour
    {
        [SerializeField] private float engineMaxSpeed;
        [SerializeField] private float engineTorque;
        [SerializeField] private float engineMOI;

        [Space]
        [FormerlySerializedAs("steerAngle")]
        [SerializeField] private float maxSteerAngle;
        [SerializeField] private float steerSpeed;

        [Space]
        [SerializeField] private Transform centerOfMass;

        [Space]
        [SerializeField] private float counterFlipScale;

        public Rigidbody Rigidbody { get; private set; }
        private List<KartWheel> wheels;
        private float engineRpm;
        
        public float ThrottlePercent { get; set; }
        public int Steer { get; set; }
        public float SteerAngle { get; set; }
        public bool SteerHeld { get; set; }
        public float BrakePercent { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (centerOfMass) Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);

            wheels = new List<KartWheel>(GetComponentsInChildren<KartWheel>());
        }

        private void FixedUpdate()
        {
            Vector3 counterFlip = Vector3.Cross(transform.up, Vector3.up) * counterFlipScale;
            Rigidbody.AddTorque(counterFlip, ForceMode.Acceleration);

            engineRpm += (ThrottlePercent * engineMaxSpeed - engineRpm) * engineTorque * Time.deltaTime;

            if (SteerHeld)
            {
                SteerAngle += (Steer * maxSteerAngle - SteerAngle) * steerSpeed * Time.deltaTime;
                SteerAngle = Mathf.Clamp(SteerAngle, -maxSteerAngle, maxSteerAngle);
            }
            else
            {
                SteerAngle = Mathf.Clamp(SteerAngle, -maxSteerAngle, maxSteerAngle);
                var turn = Vector3.Dot(transform.up, Rigidbody.angularVelocity) * Mathf.Rad2Deg;

                var newSteerAngle = SteerAngle - turn * Time.deltaTime;
                if (Mathf.Abs(newSteerAngle) < Mathf.Abs(SteerAngle)) SteerAngle = newSteerAngle;
            }

            if (BrakePercent < 0.1f)
            {
                for (int i = 0; i < 10; i++)
                {
                    foreach (var wheel in wheels)
                    {
                        if (!wheel.Drive) continue;

                        var avg = (wheel.Rpm + engineRpm * engineMOI) / (1.0f + engineMOI);
                        wheel.Rpm = avg;
                        engineRpm = avg;
                    }
                }
            }

            foreach (var wheel in wheels)
            {
                wheel.SteerAngle = SteerAngle;
                wheel.BrakePercent = BrakePercent;
            }
        }
    }
}
