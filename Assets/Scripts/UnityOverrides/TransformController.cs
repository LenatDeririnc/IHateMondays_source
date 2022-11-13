using UnityEngine;

namespace UnityOverrides
{
    public class TransformController : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        public Vector3 forward => _transform.forward;
        public Vector3 right => _transform.right;
        public Vector3 left => -_transform.right;

        private Vector3 _velocity;

        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
        }

        public void Move(Vector3 velocity)
        {
            _velocity += velocity;
        }

        public void Rotate(float angle)
        {
            _transform.Rotate(_transform.up, angle);
        }

        private void Update()
        {
            if (_velocity != Vector3.zero) {
                _transform.position += _velocity;
                _velocity = Vector3.zero;
            }
        }
    }
}