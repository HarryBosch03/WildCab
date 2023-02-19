using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using BoschingMachine.Gameplay;

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

        public Rigidbody Rigidbody { get; private set; }
        private List<KartWheel> wheels;
        private float engineRpm;
        private float engineVelocity;
        private float steerAngle;
        private List<Passenger> passengers = new();

        public float ThrottlePercent { get; set; }
        public int Steer { get; set; }
        public bool SteerHeld { get; set; }
        public float BrakePercent { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (centerOfMass) Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);

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
                var turn = Vector3.Dot(transform.up, Rigidbody.angularVelocity) * Mathf.Rad2Deg;

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

        public bool TryAddPassanger (Passenger passanger)
        {
            if (IsCarFull()) return false;

            passengers.Add(passanger);
            return true;
        }

        public bool IsCarFull ()
        {
            return passengers.Count >= 3;
        }
    }
}
