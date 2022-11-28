using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public abstract class PlayerMovementBase : MonoBehaviour
    {
        [SerializeField] protected Transform _forwardVector;
        [SerializeField] protected float _moveSpeed = 2f;
        
        protected Vector3 _movementInput;
        public Vector3 MoveDirection { get; protected set; } = Vector3.zero;
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        protected GameService _gameService;
        private FungusService _fungusService;

        protected virtual void Awake()
        {
            _gameService = ServiceLocator.Get<GameService>();
            _fungusService = ServiceLocator.Get<FungusService>();
        }
        
        protected virtual void OnDisable()
        {
            _movementInput = Vector3.zero;
            MoveDirection = Vector3.zero;
            Velocity = Vector3.zero;
        }
        
        protected virtual void UpdateMovementInput()
        {
            if (_fungusService.IsDialogue) {
                Velocity = Vector3.zero;
                MoveDirection = Vector3.zero;
                return;
            }
            
            Velocity = MovementInput();
            MoveDirection = Velocity / _moveSpeed;
        }

        protected virtual Vector3 MovementInput()
        {
            if (_forwardVector == null)
                return Vector3.forward * _movementInput.z + Vector3.right * _movementInput.x;

            return _forwardVector.forward * _movementInput.z + _forwardVector.right * _movementInput.x;
        }

        public virtual void SetMovementInput(Vector3 movement)
        {
            _movementInput = Vector3.ClampMagnitude(movement, 1f) * _moveSpeed;
        }

        public abstract void SetPosition(Vector3 position);
    }
}