using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BoschingMachine.AI;
using BoschingMachine.AI.Modules;
using BoschingMachine.Kart;

namespace BoschingMachine.Gameplay
{
    public class Passenger : MonoBehaviour
    {
        [SerializeField] float pickupRange;
        [SerializeField] float forceMagnitude;
        [SerializeField] float dampingRadius;
        [SerializeField] float dampingScale;

        [Space]
        [SerializeField] private NPCMovement movement;
        [SerializeField] private NPCAnimator animator;
        [SerializeField] private NPCRagdollinator ragdollinator;
        [Space]
        [SerializeField] private Animator characterAnimator;

        KartBody kart;
        Rigidbody part;
        bool pickedUp;

        public PassengerTarget Target { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Animator CharacterAnimator => characterAnimator;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();

            var targets = FindObjectsOfType<PassengerTarget>();

            if (targets.Length == 0)
            {
                Destroy(gameObject);
                return;
            }

            Target = targets[Random.Range(0, targets.Length)];
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (pickedUp)
            {
                FlyTowards(Target.transform.position, out bool there);
                if (there)
                {
                    Destroy(gameObject);
                }
            }
            else if (!kart)
            {
                var karts = new List<KartBody>(FindObjectsOfType<KartBody>());
                kart = karts.Where(e => !e.IsCarFull()).First(e => (e.transform.position - transform.position).sqrMagnitude < pickupRange * pickupRange);
            }
            else
            {
                if (kart.IsCarFull())
                {
                    kart = null;
                    return;
                }

                FlyTowards(kart.transform.position, out bool there);

                if (there)
                {
                    kart.Rigidbody.AddForce(part.velocity * Rigidbody.mass);
                    if (kart.TryAddPassanger(this))
                    {
                        pickedUp = true;
                        gameObject.SetActive(false);
                        part = null;
                    }
                }
            }
        }

        public void FlyTowards(Vector3 target, out bool there)
        {
            ragdollinator.Ragdoll(transform, Rigidbody, SetCollision);
            if (!part)
            {
                part = ragdollinator.Bodies.OrderBy(e => (e.position - target).sqrMagnitude).FirstOrDefault();
            }

            var vector = (target - part.position);
            var force = vector * forceMagnitude;

            float dampingAmount = Mathf.Max(dampingRadius - vector.magnitude, 0.0f) / dampingRadius;
            force += -part.velocity * dampingAmount * dampingScale;

            part.AddForce(force);

            there = vector.magnitude < 0.5f;
        }

        private void FixedUpdate()
        {
            movement.FixedUpdate(Rigidbody, Vector3.zero);
        }

        private void LateUpdate()
        {
            animator.Animate(characterAnimator, Rigidbody);
        }

        public void SetCollision(bool state)
        {
            var colliders = new List<Collider>();
            colliders.Add(GetComponent<CapsuleCollider>());

            colliders.ForEach(e => e.enabled = state);
        }
    }
}
