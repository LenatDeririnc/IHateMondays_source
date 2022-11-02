using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public abstract class PlayerGravityBase : UpdateGetter, ISelfDeps
    {
        [SerializeField] protected PlayerBase PlayerBase;
        [SerializeField] protected float GroundCheckRadius = 0.5f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] protected float jumpModifierOnButtonUp = 0.5f;

        protected bool _groundCheckEnabled = true;
        protected bool _jumpCommand;
        [SerializeField] protected float JumpHeight = 5f;
        public bool IsGrounded { get; protected set; } = false;
        
        public abstract void CutoffJump();

        protected abstract void ApplyGravity();

        public void SetJumpCommand(bool value)
        {
            _jumpCommand = value;
        }

        protected bool GroundCheck(out RaycastHit hit)
        {
            hit = new RaycastHit();
            
            var position = PlayerBase.PlayerTransform.position;

            var sphereCheck = Physics.CheckSphere(position, GroundCheckRadius, groundMask);
            var boxCheck = Physics.CheckBox(
                position + PlayerBase.PlayerTransform.up * GroundCheckRadius / 2, 
                new Vector3(GroundCheckRadius, GroundCheckRadius/2, GroundCheckRadius), 
                PlayerBase.PlayerTransform.rotation, 
                groundMask);

            var isGround = _groundCheckEnabled &&
                           sphereCheck &&
                           boxCheck;

            if (isGround)
            {
                Physics.Raycast(position + PlayerBase.PlayerTransform.up * GroundCheckRadius / 2, 
                    Vector3.down * (GroundCheckRadius / 2), out hit, (GroundCheckRadius / 2), groundMask);
            }

            return isGround;
        }

        protected override void SentFixedUpdate()
        {
            if (PlayerBase.GameService.IsPaused)
                return;
            ApplyGravity();
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

        public void SetupDeps()
        {
            PlayerBase = GetComponent<PlayerBase>();
        }
    }
}