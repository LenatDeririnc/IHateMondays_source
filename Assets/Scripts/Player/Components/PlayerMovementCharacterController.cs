using System.Collections;
using TransformTools;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerMovementCharacterController : PlayerMovementBase
    {
        [SerializeField] private float impulseScaleModifier = 0.01f;
        [SerializeField] private float impulseScaleGroundModifier = 0.5f;

        [SerializeField] private Vector3 _impulse;

        [SerializeField] private TransformVelocity _groundVelocity;
        public TransformVelocity GroundVelocity => _groundVelocity;

        private bool isIgnoreFrame = false;

        protected override void OnEnable()
        {
            _impulse = Vector3.zero;
            _movement = Vector3.zero;
        }

        public override void SetMovement(Vector3 movement)
        {
            base.SetMovement(movement);
            StopImpulse();
        }

        public void StopImpulse()
        {
            var dot = Vector3.Dot(Movement().normalized, _impulse.normalized);

            if (dot <= 0)
            {
                _impulse *= 1 + (dot * impulseScaleModifier);
            }
        }

        public void StopImpulseOnGround()
        {
            _impulse *= impulseScaleGroundModifier;
        }

        public void SetGroundVelocity(TransformVelocity value)
        {
            _groundVelocity = value;
        }

        private void GroundMovement(float deltaTime)
        {
            if (_groundVelocity == null) 
                return;

            Move(_groundVelocity.Velocity * deltaTime);
        }

        private void MovePlayer(float deltaTime)
        {
            Velocity = Movement();
            MoveVector = Velocity / MoveSpeed;
            Move(Velocity * deltaTime);
        }

        private void MovementImpulse(float deltaTime)
        {
            Move(_impulse * deltaTime);
        }

        protected override void Move(Vector3 velocity)
        {
            PlayerBase.CharacterController.Move(velocity);
        }

        protected override void SentUpdate()
        {
            if (isIgnoreFrame)
                return;
            
            if (PlayerBase.GameService.IsPaused)
                return;
            
            var delta = Time.deltaTime;
            
            MovePlayer(delta);
        }

        protected override void SentFixedUpdate()
        {
            if (isIgnoreFrame)
                return;
            
            if (PlayerBase.GameService.IsPaused)
                return;

            var delta = Time.fixedDeltaTime;

            GroundMovement(delta);
            MovementImpulse(delta);
            // MovePlayer(delta);
        }

        public void SetImpulse(Vector3 force)
        {
            _impulse = force;
            _impulse.y = 0;
        }
    }
}