using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInput_Zelda : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerMovementBase _playerMovement;

        private InputBridgeService _inputBridgeService;
        private GameService _gameService;
        private CameraService _cameraService;

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _gameService = ServiceLocator.Get<GameService>();
            _cameraService = ServiceLocator.Get<CameraService>();
        }

        public void SetupDeps()
        {
            _playerMovement = GetComponent<PlayerMovementBase>();
        }

        private void MovementInputUpdate()
        {
            var movement = _inputBridgeService.Movement;
            _playerMovement.SetMovementInput(new Vector3(movement.x, 0, movement.y));
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (!_gameService.IsPaused && !_cameraService.Brain.IsBlending)
            {
                MovementInputUpdate();
            }
            else
            {
                PausedUpdate();
            }
        }

        public void PausedUpdate()
        {
            _playerMovement.SetMovementInput(Vector3.zero);
        }
    }
}