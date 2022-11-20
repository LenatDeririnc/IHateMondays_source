using Player.Custom;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInput_FPS : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerMovementBase _playerMovement;
        [SerializeField] private PlayerLook _playerLook;
        [SerializeField] private Flashlight _flashlight;
        
        private InputBridgeService InputBridgeService;
        private GameService GameService;

        private void Awake()
        {
            InputBridgeService = ServiceLocator.Get<InputBridgeService>();
            GameService = ServiceLocator.Get<GameService>();
        }

        public void SetupDeps()
        {
            _playerMovement = GetComponent<PlayerMovementBase>();
            _playerLook = GetComponent<PlayerLook>();
        }

        private void MovementInputUpdate()
        {
            var movement = InputBridgeService.Movement;
            _playerMovement.SetMovementInput(new Vector3(movement.x, 0, movement.y));
        }

        private void RotationInputUpdate()
        {
            _playerLook.SetRotateDelta(new Vector3(InputBridgeService.Look.x, InputBridgeService.Look.y, 0));
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (!GameService.IsPaused)
            {
                MovementInputUpdate();
                RotationInputUpdate();
                FlashlightUpdate();
            }
            else
            {
                PausedUpdate();
            }
        }

        private void FlashlightUpdate()
        {
            if (InputBridgeService.IsFlashlightDown) {
                _flashlight?.SwitchActive();
            }
        }

        public void PausedUpdate()
        {
            _playerMovement.SetMovementInput(Vector3.zero);
            _playerLook.SetRotateDelta(Vector3.zero);
        }
    }
}