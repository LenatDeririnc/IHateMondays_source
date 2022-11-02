using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInput : UpdateGetter, ISelfDeps
    {

        [SerializeField] private PlayerBase PlayerBase;

        private void JumpInputUpdate()
        {
            if (PlayerBase.InputBridgeService.IsJumpUp)
            {
                PlayerBase.PlayerGravityBase.SetJumpCommand(false);
                PlayerBase.PlayerGravityBase.CutoffJump();
            }

            if (PlayerBase.InputBridgeService.IsJumpDown)
            {
                PlayerBase.PlayerGravityBase.SetJumpCommand(true);
            }
        }

        private void MovementInputUpdate()
        {
            PlayerBase.PlayerMovement.SetMovement(new Vector3(PlayerBase.InputBridgeService.Movement.x, 0, PlayerBase.InputBridgeService.Movement.y));
        }

        private void RotationInputUpdate()
        {
            PlayerBase.PlayerLook.SetRotateDelta(new Vector3(PlayerBase.InputBridgeService.Look.x, PlayerBase.InputBridgeService.Look.y, 0));
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (!PlayerBase.GameService.IsPaused)
            {
                MovementInputUpdate();
                RotationInputUpdate();
                JumpInputUpdate();
            }
            else
            {
                PausedUpdate();
            }
        }

        public void PausedUpdate()
        {
            PlayerBase.PlayerMovement.SetMovement(Vector3.zero);
            PlayerBase.PlayerLook.SetRotateDelta(Vector3.zero);
            PlayerBase.PlayerGravityBase.SetJumpCommand(false);
            PlayerBase.PlayerGravityBase.CutoffJump();
        }

        public void SetupDeps()
        {
            PlayerBase = GetComponent<PlayerBase>();
        }
    }
}