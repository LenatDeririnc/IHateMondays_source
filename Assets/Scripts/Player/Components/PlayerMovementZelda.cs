using System;
using Player.Components;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerMovementZelda : PlayerMovementBase, ISelfDeps
    {
        [SerializeField] protected CharacterControllerAccelerator characterController;
        [SerializeField] protected Transform mesh;
        private CameraService _cameraService;

        public void SetupDeps()
        {
            characterController = GetComponent<CharacterControllerAccelerator>();
        }

        protected override void Awake()
        {
            base.Awake();
            _cameraService = ServiceLocator.Get<CameraService>();
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
        }

        public override void SetPosition(Vector3 position)
        {
            characterController.transform.position = position;
        }
    }
}