using Characters.Components;
using UnityEngine;

namespace Player.Components
{
    public class PlayerMovementRigidbody : PlayerMovementBase
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private GroundCheckComponent _groundCheckComponent;
        public override void SetPosition(Vector3 position)
        {
            _transform.position = position;
        }

        private void Update()
        {
            UpdateMovementInput();
        }

        private void FixedUpdate()
        {
            MoveProcess(Time.fixedDeltaTime);
        }

        private void UpdateKinematic(bool value)
        {
            if (value) {
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        protected override void MoveProcess(float delta)
        {
            if (_groundCheckComponent.IsGrounded && Velocity.magnitude <= 0) {
                UpdateKinematic(true);
                _transform.position = 
                    new Vector3(_transform.position.x,
                    _groundCheckComponent.LastHit.point.y,
                    _transform.position.z);
                return;
            }
            
            UpdateKinematic(false);
            _rigidbody.velocity = new Vector3(Velocity.x, _rigidbody.velocity.y, Velocity.z);
        }
    }
}