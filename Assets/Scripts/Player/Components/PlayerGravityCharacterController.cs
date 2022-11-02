using System.Collections;
using TransformTools;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerGravityCharacterController : PlayerGravityBase
    {
        PlayerMovementCharacterController PlayerMovement =>
            (PlayerMovementCharacterController)PlayerBase.PlayerMovement;
        
        [SerializeField] private float maxFallSpeed = 8;
        [SerializeField] private float TimeScale = 1;
        [SerializeField] private float _groundedFallSpeed = -1f;

        private float _yVelocity = 0f;
        
        private void OnEnable()
        {
            _yVelocity = 0f;
            _groundCheckEnabled = true;
            _jumpCommand = false;
        }

        public override void CutoffJump()
        {
            if (_yVelocity > 0)
                _yVelocity *= jumpModifierOnButtonUp;
        }

        private Collider previousGround;

        private Vector3 Gravity(float time)
        {
            if (_yVelocity <= 0)
            {
                _groundCheckEnabled = true;
            }

            var groundCheck = GroundCheck(out var hit);

            IsGrounded = groundCheck;

            if (IsGrounded)
            {
                _yVelocity = _groundedFallSpeed;
                PlayerMovement.StopImpulseOnGround();

                if (hit.collider != null)
                {
                    TransformVelocity data = hit.collider.GetComponent<TransformVelocity>();
                    PlayerMovement.SetGroundVelocity(data);

                    if (previousGround != hit.collider)
                    {
                        PlayerBase.PlayerWalksteps.PlayGrounded();
                        previousGround = hit.collider;
                    }
                }
            }
            else
            {
                PlayerMovement.SetGroundVelocity(null);
            }

            if (IsGrounded && _jumpCommand)
            {
                if (PlayerMovement.GroundVelocity != null)
                {
                    PlayerMovement.SetImpulse(PlayerMovement.GroundVelocity.Velocity);
                }

                _yVelocity = JumpHeight;
                _jumpCommand = false;
                _groundCheckEnabled = false;
                PlayerBase.PlayerWalksteps.PlayJump();
                previousGround = null;
            }
            else
            {
                _yVelocity =
                    Mathf.Clamp(
                        _yVelocity - Physics.gravity.magnitude * TimeScale * time,
                        -maxFallSpeed,
                        Mathf.Infinity
                    );
            }

            return new Vector3(0, _yVelocity, 0) * TimeScale;
        }

        protected override void ApplyGravity()
        {
            PlayerBase.CharacterController.Move(Gravity(Time.fixedDeltaTime) * Time.fixedDeltaTime);
        }

        public void SetVelocityY(float velocity)
        {
            _yVelocity = velocity;
        }
    }
}