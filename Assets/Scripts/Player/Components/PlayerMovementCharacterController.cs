using Player.Components;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using TransformTools;
using UnityEngine;
using UnityOverrides;

namespace Characters.Components
{
    public class PlayerMovementCharacterController : PlayerMovementBase, ISelfDeps
    {
        [SerializeField] protected PlayerForwardTransform _playerForwardTransform;
        [SerializeField] private CharacterControllerDecorator _characterControllerAccelerator;
        [SerializeField] private float impulseScaleModifier = 0.01f;
        [SerializeField] private float impulseScaleGroundModifier = 0.5f;

        [SerializeField] private Vector3 _impulse;

        [SerializeField] private TransformVelocity _groundVelocity;
        public CharacterController CharacterController => _characterControllerAccelerator.CharacterController;

        private GameService GameService;
        
        public TransformVelocity GroundVelocity => _groundVelocity;

        private bool isIgnoreFrame = false;

        private void Awake()
        {
            ServiceLocator.Get(ref GameService);
        }

        public void SetupDeps()
        {
            _playerForwardTransform = GetComponent<PlayerForwardTransform>();
            _characterControllerAccelerator = GetComponent<CharacterControllerDecorator>();
        }
        
        protected override Vector3 MovementInput()
        {
            return _playerForwardTransform.Value.forward * _movementInput.z + _playerForwardTransform.Value.right * _movementInput.x;
        }

        protected override void OnEnable()
        {
            _impulse = Vector3.zero;
            _movementInput = Vector3.zero;
        }

        public override void SetMovementInput(Vector3 movement)
        {
            base.SetMovementInput(movement);
            StopImpulse();
        }

        public void StopImpulse()
        {
            var dot = Vector3.Dot(MovementInput().normalized, _impulse.normalized);

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

        private void MovementImpulse(float deltaTime)
        {
            Move(_impulse * deltaTime);
        }

        protected override void Move(Vector3 velocity)
        {
            _characterControllerAccelerator.Move(velocity);
        }

        protected override void SentUpdate()
        {
            if (isIgnoreFrame)
                return;
            
            if (GameService.IsPaused)
                return;
            
            var delta = Time.deltaTime;
            
            MovePlayer(delta);
        }

        protected override void SentFixedUpdate()
        {
            if (isIgnoreFrame)
                return;
            
            if (GameService.IsPaused)
                return;

            var delta = Time.fixedDeltaTime;

            GroundMovement(delta);
            MovementImpulse(delta);
        }

        public void SetImpulse(Vector3 force)
        {
            _impulse = force;
            _impulse.y = 0;
        }
    }
}