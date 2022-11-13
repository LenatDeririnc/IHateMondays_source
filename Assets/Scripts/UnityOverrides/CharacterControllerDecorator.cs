using UnityEngine;

namespace UnityOverrides
{
    public class CharacterControllerDecorator : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _forward;

        public Transform Transform => _forward;

        public CharacterController CharacterController => _characterController;


        private Vector3 _setPosition;

        public void SetPosition(Vector3 position)
        {
            _characterController.Move(position - _characterController.transform.position);
        }

        public void SetRotation(Quaternion rotation)
        {
            _forward.rotation = rotation;
        }

        public void Move(Vector3 move)
        {
            _characterController.Move(move);
        }

        public void Rotate(Vector3 transformUp, int angle)
        {
            _forward.Rotate(transformUp, angle);
        }
    }
}