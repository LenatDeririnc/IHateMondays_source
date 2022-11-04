using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInput_Zelda : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerMovementBase _playerMovement;

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
        }

        private void MovementInputUpdate()
        {
            var movement = InputBridgeService.Movement;
            _playerMovement.SetMovementInput(new Vector3(movement.x, 0, movement.y));
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (!GameService.IsPaused)
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