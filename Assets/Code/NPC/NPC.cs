using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.AI.Modules;

namespace BoschingMachine.AI
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private NPCMovement movement;
        [SerializeField] private NPCAnimator animator;
        [SerializeField] private NPCRagdollinator ragdollinator;
        [SerializeField] private float maxRagdollMomentum;
        
        private new Rigidbody rigidbody;
        private Vector2 moveDirection;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            movement.FixedUpdate(rigidbody, moveDirection);
        }

        private void LateUpdate()
        {
            animator.Animate(rigidbody);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.rigidbody) return;
            
            var netMomentum = collision.relativeVelocity * (rigidbody.mass + collision.rigidbody.mass);
            var momentumLength = netMomentum.magnitude;
            
            if (!(momentumLength > maxRagdollMomentum)) return;
            
            ragdollinator.Ragdoll(this);
            Destroy(gameObject);
        }
    }
}
