using UnityEngine;

namespace BoschingMachine.Kart
{
    public class KartWheel : MonoBehaviour
    {
        [SerializeField] private float wheelRadius = 0.3f;

        [Header("Friction")] [SerializeField] private bool steer;
        [Header("Friction")] [SerializeField] private bool drive;
        [SerializeField] private float rollingFrictionScale;
        [SerializeField] private float tangentialFrictionScale;
        [SerializeField] private AnimationCurve slipFrictionCurve;
        [SerializeField] private float slip;
        
        [Header("Suspension")]
        [SerializeField] private float suspensionRange = 0.3f;
        [SerializeField] private float springFrequency = 80000.0f;
        [SerializeField] private float springDamper = 4500.0f;

        private new Rigidbody rigidbody;
        
        private float groundDistance;
        private Rigidbody ground;
        private Vector3 contactPoint;
        
        private Vector3 targetVelocity;

        public bool Grounded => groundDistance < suspensionRange;
        
        public float RPM { get; set; }
        public float SteerAngle { get; set; }

        private void Awake()
        {
            rigidbody = GetComponentInParent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            CheckForGround();

            CalculateTargetVelocity();
            CalculateSlip();

            ApplySuspension();
            ApplyFriction();
        }

        private void CalculateTargetVelocity()
        {
            targetVelocity = Vector3.zero;

            if (drive)
            {
                
            }
            else
            {
                
            }
            var actualVelocity = rigidbody.GetPointVelocity(contactPoint);
            targetVelocity += transform.forward * Vector3.Dot(transform.forward, actualVelocity);
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
        
            var otherVelocity = ground ? ground.velocity : Vector3.zero;

            var relativeVelocity = targetVelocity - otherVelocity;
            var frictionScale = slipFrictionCurve.Evaluate(slip);
            var rollingFriction = Vector3.Dot(transform.forward, relativeVelocity) * frictionScale * rollingFrictionScale;
            var tangentialFriction = Vector3.Dot(transform.right, relativeVelocity) * frictionScale * tangentialFrictionScale;

            var friction = transform.forward * rollingFriction + transform.right * tangentialFriction;
            
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
