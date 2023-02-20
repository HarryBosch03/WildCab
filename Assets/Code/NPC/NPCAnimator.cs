using UnityEngine;

namespace BoschingMachine.AI.Modules
{
    [System.Serializable]
    public class NPCAnimator
    {
        public void Animate(Animator animator, Rigidbody rigidbody)
        {
            var planarVelocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
            var planarSpeed = planarVelocity.magnitude;
            
            animator.SetFloat("moveSpeed", planarSpeed);
        }
    }
}
