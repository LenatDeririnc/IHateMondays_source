using UnityEngine;

namespace TransformTools
{
    public class TransformVelocity : MonoBehaviour
    {
        [SerializeField] private bool ShowDebugLog;
        [SerializeField] private Transform Transform;
        private Vector3 _lastPosition;
        private Vector3 _velocity;

        public Vector3 Velocity => _velocity;

        private void Start()
        {
            _lastPosition = Transform.position;
        }

        private void UpdateVelocity(float deltaTime)
        {
            _velocity = (Transform.position - _lastPosition) / deltaTime;
            _lastPosition = Transform.position;
            
            if (ShowDebugLog)
            {
                Debug.Log($"Velocity of {Transform.name} = {_velocity}");
            }
        }

        public void FixedUpdate()
        {
            UpdateVelocity(Time.fixedDeltaTime);
        }
    }
}