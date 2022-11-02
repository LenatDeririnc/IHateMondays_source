using System.Collections;
using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public abstract class PlayerMovementBase : UpdateGetter, ISelfDeps
    {
        [SerializeField] protected PlayerBase PlayerBase;
        [SerializeField] protected float MoveSpeed = 2f;
        [SerializeField] protected Vector3 _movement;
        public Vector3 MoveVector { get; protected set; } = Vector3.zero;
        public Vector3 Velocity { get; protected set; } = Vector3.zero;

        protected Vector3 Movement()
        {
            return PlayerBase.PlayerTransform.forward * _movement.z + PlayerBase.PlayerTransform.right * _movement.x;
        }

        public virtual void SetMovement(Vector3 movement)
        {
            _movement = movement * MoveSpeed;
        }

        protected abstract void Move(Vector3 velocity);
        public void SetupDeps()
        {
            PlayerBase = GetComponent<PlayerBase>();
        }
    }
}