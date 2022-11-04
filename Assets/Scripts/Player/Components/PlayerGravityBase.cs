using System;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public abstract class PlayerGravityBase : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerTransform PlayerTransform;
        
        [SerializeField] protected float GroundCheckRadius = 0.5f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] protected float jumpModifierOnButtonUp = 0.5f;
        [SerializeField] protected float JumpHeight = 5f;

        protected bool _groundCheckEnabled = true;
        protected bool _jumpCommand;

        private GameService GameService;
        
        public bool IsGrounded { get; protected set; } = false;
        
        public abstract void CutoffJump();

        protected abstract void ApplyGravity();

        private void Awake()
        { 
            ServiceLocator.Get(ref GameService);
        }

        public void SetJumpCommand(bool value)
        {
            _jumpCommand = value;
        }

        protected bool GroundCheck(out RaycastHit hit)
        {
            hit = new RaycastHit();
            
            var position = PlayerTransform.Value.position;

            var sphereCheck = Physics.CheckSphere(position, GroundCheckRadius, groundMask);
            var boxCheck = Physics.CheckBox(
                position + PlayerTransform.Value.up * GroundCheckRadius / 2, 
                new Vector3(GroundCheckRadius, GroundCheckRadius/2, GroundCheckRadius), 
                PlayerTransform.Value.rotation, 
                groundMask);

            var isGround = _groundCheckEnabled &&
                           sphereCheck &&
                           boxCheck;

            if (isGround)
            {
                Physics.Raycast(position + PlayerTransform.Value.up * GroundCheckRadius / 2, 
                    Vector3.down * (GroundCheckRadius / 2), out hit, (GroundCheckRadius / 2), groundMask);
            }

            return isGround;
        }

        protected override void SentFixedUpdate()
        {
            if (GameService.IsPaused)
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

        public virtual void SetupDeps()
        {
            PlayerTransform = GetComponent<PlayerTransform>();
        }
    }
}