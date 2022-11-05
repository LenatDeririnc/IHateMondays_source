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
        private GameService _gameService;
        private CameraService _cameraService;

        public void SetupDeps()
        {
            characterController = GetComponent<CharacterControllerAccelerator>();
        }

        protected override void Awake()
        {
            base.Awake();
            _gameService = ServiceLocator.Get<GameService>();
            _cameraService = ServiceLocator.Get<CameraService>();
        }

        protected override void Move(Vector3 velocity)
        {
            characterController.Move(velocity);
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
        
        protected override void SentUpdate()
        {
            if (_gameService.IsPaused)
                return;
            
            if (_cameraService.Brain.IsBlending)
                return;
            
            var delta = Time.deltaTime;
            
            MovePlayer(delta);
        }
    }
}