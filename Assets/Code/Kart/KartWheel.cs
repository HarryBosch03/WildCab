using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoschingMachine.Kart
{
    public class KartWheel : MonoBehaviour
    {
        [SerializeField] private float wheelRadius = 0.3f;

        [Header("Function")]
        [SerializeField] private bool steer;
        [SerializeField] private bool drive;
        [SerializeField] private bool brake;

        [Header("Friction")] 
        [SerializeField] private float frictionScale;
        [SerializeField] private AnimationCurve slipFrictionCurve;
        [SerializeField] private float slip;
        
        [Header("Suspension")]
        [SerializeField] private float suspensionRange = 0.3f;
        [SerializeField] private float springFrequency = 80000.0f;
        [SerializeField] private float springDamper = 4500.0f;

        [Header("Visuals")] 
        [SerializeField] private Transform visualDriver;

        private new Rigidbody rigidbody;
        private float rotation;
        
        private float groundDistance;
        private Rigidbody ground;
        private Vector3 contactPoint;
        
        private Vector3 targetVelocity;

        public bool Grounded => groundDistance < suspensionRange;
        
        public float Rpm { get; set; }
        public float SteerAngle { get; set; }
        public float BrakePercent { get; set; }

        private void Awake()
        {
            rigidbody = GetComponentInParent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            CheckForGround();

            CalculateTargetVelocity();
            CalculateSlip();

            ApplySteering();
            ApplySuspension();
            ApplyFriction();
        }

        private void Update()
        {
            rotation += Rpm * 6.0f * Time.deltaTime;
            rotation %= 360.0f;

            visualDriver.transform.position = contactPoint + Vector3.up * (groundDistance + wheelRadius);
            visualDriver.transform.rotation = transform.rotation * Quaternion.Euler(transform.right * rotation);
        }

        private void ApplySteering()
        {
            if (!steer) return;
            
            transform.localRotation = Quaternion.Euler(transform.up * SteerAngle);
        }

        private void CalculateTargetVelocity()
        {
            targetVelocity = Vector3.zero;

            var actualVelocity = rigidbody.GetPointVelocity(contactPoint);
            targetVelocity += transform.forward * Vector3.Dot(transform.forward, actualVelocity);
            
            if (drive)
            {
                targetVelocity -= transform.forward * Vector3.Dot(transform.forward, targetVelocity);
                var wheelSpeed = Rpm * Mathf.PI * wheelRadius / 30.0f;
                targetVelocity += transform.forward * wheelSpeed;
            }
            if (brake)
            {
                targetVelocity -= transform.forward * (Vector3.Dot(transform.forward, targetVelocity) * BrakePercent);
            }
            if (ground)
            {
                targetVelocity += ground.velocity;
            }
            
            Debug.DrawRay(contactPoint, actualVelocity, Color.green);
            Debug.DrawRay(contactPoint, targetVelocity, Color.red);
        }


        private void CalculateSlip()
        {
            if (!Grounded) return;
            
            var actualVelocity = rigidbody.GetPointVelocity(contactPoint);
            var dot = Vector3.Dot(targetVelocity.normalized, actualVelocity.normalized);
            slip = (0.5f - dot * 0.5f);
        }

        private void ApplyFriction()
        {
            if (!Grounded) return;

            var diff = targetVelocity - rigidbody.GetPointVelocity(contactPoint);
            var friction = diff * (slipFrictionCurve.Evaluate(slip) * this.frictionScale);

            friction.y = 0.0f;
            rigidbody.AddForceAtPosition(friction, contactPoint);
        }

        private void ApplySuspension()
        {
            if (!Grounded) return;
            
            var springForce = -groundDistance * springFrequency;

            var velAtPoint = rigidbody.GetPointVelocity(transform.position);
            var damper = -Vector3.Dot(velAtPoint, Vector3.up) * springDamper;

            springForce += damper;
            rigidbody.AddForceAtPosition(Vector3.up * springForce, contactPoint);
        }

        private void CheckForGround()
        {
            var ray = new Ray(transform.position + Vector3.up * wheelRadius, Vector3.down);
            if (Physics.SphereCast(ray, wheelRadius, out var hit))
            {
                groundDistance = hit.distance - wheelRadius - suspensionRange;
                ground = hit.rigidbody;
                contactPoint = hit.point;
            }
            else
            {
                groundDistance = float.PositiveInfinity;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;

            var iterations = 16;
            for (var i = 0; i < iterations; i++)
            {
                var a1 = i / (float)iterations * Mathf.PI * 2.0f;
                var a2 = (i + 1.0f) / iterations * Mathf.PI * 2.0f;

                var p1 = new Vector3(0.0f, Mathf.Sin(a1), Mathf.Cos(a1)) * wheelRadius;
                var p2 = new Vector3(0.0f, Mathf.Sin(a2), Mathf.Cos(a2)) * wheelRadius;

                Gizmos.DrawLine(p1, p2);
            }
            
            Gizmos.color = Color.white;
        }
    }
}
