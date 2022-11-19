using UnityEngine;

namespace Characters.Components
{
    public class GroundCheckComponent : MonoBehaviour
    {
        [SerializeField] private PlayerForwardTransform playerForwardTransform;
        [SerializeField] protected float GroundCheckRadius = 0.5f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float SlopeHitDistance;
        RaycastHit _lastHit;
        
        public RaycastHit LastHit => _lastHit;
        public bool IsGrounded => GroundCheck();
        
        protected bool GroundCheck()
        {
            var position = playerForwardTransform.Value.position;

            // var sphereCheck = Physics.CheckSphere(position, GroundCheckRadius, groundMask);
            // var boxCheck = Physics.CheckBox(
            //     position + playerForwardTransform.Value.up * GroundCheckRadius / 2, 
            //     new Vector3(GroundCheckRadius, GroundCheckRadius/2, GroundCheckRadius), 
            //     playerForwardTransform.Value.rotation, 
            //     groundMask);
            
            var raycast = Physics.Raycast(position + playerForwardTransform.Value.up * GroundCheckRadius / 2, 
                Vector3.down * (GroundCheckRadius / 2), out _lastHit, SlopeHitDistance, groundMask);

            var isGround = 
                // sphereCheck && 
                // boxCheck && 
                raycast;

            return isGround;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var transform1 = transform;
            var position = transform1.position;
            var forward = transform1.forward;
            var right = transform1.right;
            
            Gizmos.DrawSphere(position, 0.1f);
            Gizmos.DrawLine(position + forward * 1, position + right * 1);
            Gizmos.DrawLine(position + forward * 1, position + right * -1);
            
            Gizmos.DrawWireSphere(position, GroundCheckRadius);
        }
    }
}