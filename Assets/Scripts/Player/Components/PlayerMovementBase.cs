using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public abstract class PlayerMovementBase : UpdateGetter
    {
        
        [SerializeField] protected float _moveSpeed = 2f;
        [SerializeField] protected Vector3 _movementInput;
        public Vector3 MoveVector { get; protected set; } = Vector3.zero;
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        private InputBridgeService InputBridgeService;
        
        protected virtual void Awake()
        {
            ServiceLocator.Get(ref InputBridgeService);
        }

        protected virtual Vector3 MovementInput()
        {
            return Vector3.forward * _movementInput.z + Vector3.right * _movementInput.x;
        }

        public virtual void SetMovementInput(Vector3 movement)
        {
            _movementInput = movement * _moveSpeed;
        }
        
        protected virtual void MovePlayer(float deltaTime)
        {
            Velocity = MovementInput();
            MoveVector = Velocity / _moveSpeed;
            Move(Velocity * deltaTime);
        }

        protected abstract void Move(Vector3 velocity);
    }
}