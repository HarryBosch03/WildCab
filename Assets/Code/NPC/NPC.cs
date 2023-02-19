using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BoschingMachine.AI.Modules;

namespace BoschingMachine.AI
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private NPCMovement movement;
        [SerializeField] private NPCAnimator animator;
        [SerializeField] private NPCRagdollinator ragdollinator;
        [SerializeField] private float maxRagdollMomentum;
        [SerializeField] private float maxRagdollTime;

        [Space]
        [SerializeField] NPCBehaviour behaviour;

        [Space]
        [SerializeField] private Animator characterAnimator;

        public Rigidbody Rigidbody { get; private set; }
        public Animator CharacterAnimator => characterAnimator;
        private Vector3 moveDirection;
        private float ragdollTimer;

        public Vector3 FaceDirection { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {

        }

        private void Update()
        {
            moveDirection = Vector3.zero;

            if (ragdollinator.Ragdolled)
            {
                ragdollinator.Recenter(transform);

                if (ragdollinator.IsRagdollStill()) ragdollTimer += Time.deltaTime;
                else ragdollTimer = 0.0f;
            }
            else
            {
                ragdollTimer = 0.0f;
                behaviour.Update(this);
            }

            if (ragdollTimer > maxRagdollTime)
            {
                ragdollinator.Resume(transform, Rigidbody, SetCollision);
            }

            if (FaceDirection.sqrMagnitude > 0.1f * 0.1f)
            {
                float a = Mathf.Atan2(FaceDirection.x, FaceDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, a, 0.0f);
            }
        }

        private void FixedUpdate()
        {
            movement.FixedUpdate(Rigidbody, moveDirection);
        }

        private void LateUpdate()
        {
            animator.Animate(characterAnimator, Rigidbody);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.rigidbody) return;

            var netMomentum = collision.relativeVelocity * (Rigidbody.mass + collision.rigidbody.mass);
            var momentumLength = netMomentum.magnitude;

            if (!(momentumLength > maxRagdollMomentum)) return;

            ragdollinator.Ragdoll(transform, Rigidbody, SetCollision);
        }

        public void SetCollision(bool state)
        {
            var colliders = new List<Collider>();
            colliders.Add(GetComponent<CapsuleCollider>());

            colliders.ForEach(e => e.enabled = state);
        }

        public void MoveAndFacePoint(Vector3 point)
        {
            MoveToPoint(point);
            FacePoint(point);
        }
        
        public void MoveToPoint(Vector3 point)
        {
            var dir = point - transform.position;
            if (dir.sqrMagnitude < 1.0f) moveDirection = Vector3.zero;
            else moveDirection = dir.normalized;
        }

        public void FacePoint (Vector3 point)
        {
            FaceDirection = point - transform.position;
        }
    }
}
;