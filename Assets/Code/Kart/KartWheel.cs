using UnityEngine;

namespace BoschingMachine.Kart
{
    public class KartWheel : MonoBehaviour
    {
        [SerializeField] private float wheelRadius = 0.3f;
        [SerializeField] private float wheelMOI = 0.3f;

        [Header("Function")]
        [SerializeField] private bool drive;
        [SerializeField] private bool steer;
        [SerializeField] private bool brake;
        [SerializeField] private float brakeForce;

        [Header("Friction")] 
        [SerializeField] private float frictionScale;
        [SerializeField] private AnimationCurve slipFrictionCurve;
        [SerializeField] private float slipScale;
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

        public bool Drive => drive;
        public bool Steer => steer;
        public bool Brake => brake;

        public float Rpm { get; set; }
        public float SteerAngle { get; set; }
        public float BrakePercent { get; set; }

        private void Awake()
        {
            rigidbody = GetComponentInParent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (brake) Rpm = Mathf.MoveTowards(Rpm, 0.0f, BrakePercent * brakeForce * Time.deltaTime);

            CheckForGround();

            CalculateTargetVelocity();
            CalculateSlip();

            ApplySteering();
            ApplySuspension();
            ApplyFriction();

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            rotation += (Rpm) * 6.0f * Time.deltaTime;
            rotation %= 360.0f;

            visualDriver.transform.position = transform.position + -transform.up * (suspensionRange + Mathf.Min(groundDistance, 0.0f));
            visualDriver.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.right * rotation);
        }

        private void ApplySteering()
        {
            if (!steer) return;
            
            transform.localRotation = Quaternion.Euler(transform.up * SteerAngle);
        }

        private void CalculateTargetVelocity()
        {
            targetVelocity = transform.forward * Rpm * Mathf.PI * wheelRadius / 30.0f;
            targetVelocity *= 1.0f - BrakePercent;

            if (ground)
            {
                targetVelocity += ground.velocity;
            }
        }

        private void CalculateSlip()
        {
            if (!Grounded) return;
            
            var actualVelocity = rigidbody.GetPointVelocity(contactPoint);
            var diff = (targetVelocity - actualVelocity).magnitude;
            diff /= Mathf.Max(targetVelocity.magnitude, actualVelocity.magnitude);
            slip = float.IsNaN(diff) ? 0.0f : diff;
        }

        private void ApplyFriction()
        {
            if (!Grounded) return;

            var diff = targetVelocity - rigidbody.GetPointVelocity(contactPoint);
            var friction = diff * (slipFrictionCurve.Evaluate(slip) * frictionScale);
            friction -= transform.up * Vector3.Dot(friction, transform.up);

            Rpm -= Vector3.Dot(transform.forward, friction) * wheelRadius / wheelMOI * Time.deltaTime;

            rigidbody.AddForceAtPosition(friction, contactPoint);
        }

        private void ApplySuspension()
        {
            if (!Grounded) return;
            
            var springForce = -groundDistance * springFrequency;

            var velAtPoint = rigidbody.GetPointVelocity(transform.position);
            var damper = -Vector3.Dot(velAtPoint, transform.up) * springDamper;

            springForce += damper;
            rigidbody.AddForceAtPosition(transform.up * springForce, contactPoint);
        }

        private void CheckForGround()
        {
            var ray = new Ray(transform.position + transform.up * wheelRadius, -transform.up);
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

            const int iterations = 16;
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
