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

        private InputBridgeService _inputBridgeService;
        private FungusService _fungusService;

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _fungusService = ServiceLocator.Get<FungusService>();
        }

        public void SetupDeps()
        {
            _playerMovement = GetComponent<PlayerMovementBase>();
            _playerLook = GetComponent<PlayerLook>();
        }

        private void MovementInputUpdate()
        {
            var movement = _inputBridgeService.Movement;
            _playerMovement.SetMovementInput(new Vector3(movement.x, 0, movement.y));
        }

        private void RotationInputUpdate()
        {
            _playerLook.SetRotateDelta(new Vector3(_inputBridgeService.Look.x, _inputBridgeService.Look.y, 0));
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (!_fungusService.IsDialogue)
            {
                MovementInputUpdate();
                RotationInputUpdate();
                // FlashlightUpdate();
            }
            else
            {
                PausedUpdate();
            }
        }

        // private void FlashlightUpdate()
        // {
        //     if (InputBridgeService.IsFlashlightDown) {
        //         _flashlight?.SwitchActive();
        //     }
        // }

        public void PausedUpdate()
        {
            _playerMovement.SetMovementInput(Vector3.zero);
            _playerLook.SetRotateDelta(Vector3.zero);
        }
    }
}