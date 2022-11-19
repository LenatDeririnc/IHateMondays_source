using Characters.Components;
using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Player.Custom
{
    public class FlashlightMovement : UpdateGetter
    {
        private const string Movement = "Movement";
        private const string MovementSpeed = "MovementSpeed";
        private static readonly int Speed = Animator.StringToHash(MovementSpeed);

        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private PlayerMovementBase _characterController;
        private int _index;

        private void Awake()
        {
            _index = cameraAnimator.GetLayerIndex(Movement);
        }

        protected override void SentUpdate()
        {
            cameraAnimator.SetFloat(Speed, _characterController.MoveDirection.magnitude);
            cameraAnimator.SetLayerWeight(_index, _characterController.MoveDirection.magnitude);
        }
    }
}