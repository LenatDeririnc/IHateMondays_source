using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public abstract class PlayerMovementBase : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerTransform PlayerTransform;
        [SerializeField] protected float MoveSpeed = 2f;
        [SerializeField] protected Vector3 _movement;
        public Vector3 MoveVector { get; protected set; } = Vector3.zero;
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        public virtual void SetupDeps()
        {
            PlayerTransform = GetComponent<PlayerTransform>();
        }

        protected Vector3 Movement()
        {
            return PlayerTransform.Value.forward * _movement.z + PlayerTransform.Value.right * _movement.x;
        }

        public virtual void SetMovement(Vector3 movement)
        {
            _movement = movement * MoveSpeed;
        }

        protected abstract void Move(Vector3 velocity);
    }
}