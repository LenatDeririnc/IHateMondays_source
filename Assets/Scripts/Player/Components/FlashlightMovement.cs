using Characters.Components;
using UnityEngine;

namespace Player.Custom
{
    public class FlashlightMovement : MonoBehaviour
    {
        private const string Movement = "Movement";
        private const string MovementSpeed = "MovementSpeed";
        private static readonly int Speed = Animator.StringToHash(MovementSpeed);

        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private PlayerMovementBase _characterController;
        [SerializeField] private Transform _flashlightTransform;
        [SerializeField] private Transform _flashlightTargetTransform;
        [SerializeField] private float _lerpFlashlightAnimation = 1000f;
        private int _index;

        private void Awake()
        {
            _index = cameraAnimator.GetLayerIndex(Movement);
        }

        protected void FixedUpdate()
        {
            cameraAnimator.SetFloat(Speed, _characterController.MoveDirection.magnitude);
            cameraAnimator.SetLayerWeight(_index, _characterController.MoveDirection.magnitude);
            
            _flashlightTransform.position = Vector3.Lerp(_flashlightTransform.position,
                _flashlightTargetTransform.position, _lerpFlashlightAnimation * Time.fixedDeltaTime);
            
            _flashlightTransform.rotation = Quaternion.Lerp(_flashlightTransform.rotation,
                _flashlightTargetTransform.rotation, _lerpFlashlightAnimation * Time.fixedDeltaTime);
        }
    }
}