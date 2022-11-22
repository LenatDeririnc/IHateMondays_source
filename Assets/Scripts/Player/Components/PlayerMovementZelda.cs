using Player.Components;
using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerMovementZelda : PlayerMovementBase, ISelfDeps
    {
        [SerializeField] protected CharacterControllerAccelerator characterController;
        [SerializeField] protected Transform mesh;
        [SerializeField] protected Animator animator;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Walkspeed = Animator.StringToHash("WalkSpeed");

        public void SetupDeps()
        {
            characterController = GetComponent<CharacterControllerAccelerator>();
        }

        protected void MoveProcess(float delta)
        {
            characterController.Move(Velocity * delta);
            Vector3 controllerVelocity = characterController.CharacterController.velocity;
            switch (controllerVelocity.magnitude) {
                case > 0:
                    mesh.rotation = Quaternion.LookRotation(controllerVelocity);
                    return;
                case <= 0 when _movementInput.magnitude > 0:
                    mesh.rotation = Quaternion.LookRotation(_movementInput);
                    break;
            }
        }

        private void Update()
        {
            UpdateMovementInput();
            MoveProcess(Time.deltaTime);
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            animator.SetBool(IsWalking, Velocity.magnitude > 0);
            animator.SetFloat(Walkspeed, MoveDirection.magnitude);
        }

        public override void SetPosition(Vector3 position)
        {
            characterController.transform.position = position;
        }
    }
}