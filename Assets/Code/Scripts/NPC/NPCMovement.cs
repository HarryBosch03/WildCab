using UnityEngine;

namespace BoschingMachine.AI.Modules
{
    [System.Serializable]
    public class NPCMovement
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float acceleration;

        public void FixedUpdate(Rigidbody rigidbody, Vector3 moveDirection)
        {
            var target = moveDirection * moveSpeed;
            var diff = target - rigidbody.velocity;
            diff.y = 0.0f;
            diff = Vector3.ClampMagnitude(diff, moveSpeed);
            
            rigidbody.AddForce(diff * acceleration, ForceMode.Acceleration);
            
        }
    }
}
