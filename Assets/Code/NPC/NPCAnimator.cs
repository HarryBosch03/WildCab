using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.AI.Modules
{
    [System.Serializable]
    public class NPCAnimator
    {
        [SerializeField] private Animator animator;

        public void Animate(Rigidbody rigidbody)
        {
            var planarVelocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
            var planarSpeed = planarVelocity.magnitude;
            
            animator.SetFloat("moveSpeed", planarSpeed);
        }
    }
}
