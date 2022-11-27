using UnityEngine;

namespace Tools
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _speed;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            _transform.Rotate(_speed * Time.fixedDeltaTime);
        }
    }
}