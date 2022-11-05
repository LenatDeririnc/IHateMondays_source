using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using UnityOverrides;

namespace Characters.Components
{
    public class PlayerMovementZelda : PlayerMovementBase, ISelfDeps
    {
        [SerializeField] protected CharacterControllerDecorator characterController;
        [SerializeField] protected Transform mesh;
        private GameService _gameService;
        private CameraService _cameraService;

        public void SetupDeps()
        {
            characterController = GetComponent<CharacterControllerDecorator>();
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
            if (controllerVelocity.magnitude > 0) {
                mesh.rotation = Quaternion.LookRotation(controllerVelocity);
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