using Player;
using Player.Custom;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class FPSController : PlayerBase
    {
        public PlayerMovementBase PlayerMovementBase;
        public PlayerInput_FPS playerInputFPS;
        public PlayerLook PlayerLook;
        public PlayerWalksteps PlayerWalksteps;
        public PlayerInteract PlayerInteract;
        public FlashlightMovement FlashlightMovement;
        
        private SceneLoadingService SceneLoadingService;

        public void Awake()
        {
            SceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            SceneLoadingService.OnLoadingStart += OnLoading;
        }

        private void OnDestroy()
        {
            SceneLoadingService.OnLoadingStart -= OnLoading;
        }

        public void Update()
        {
            PlayerWalksteps.UpdateInvoke();
            playerInputFPS.UpdateInvoke();
            PlayerLook.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
            FlashlightMovement.UpdateInvoke();
        }

        public void OnLoading()
        {
            PlayerMovementBase.enabled = false;
            PlayerInteract.enabled = false;
            PlayerLook.enabled = false;
        }

        public override void SetPositionAndRotation(Transform positionToRespawn)
        {
            SetPosition(positionToRespawn.position);
            SetRotation(positionToRespawn.rotation);
        }

        public override void SetPosition(Vector3 position)
        {
            PlayerMovementBase.SetPosition(position);
        }

        public override void SetRotation(Quaternion rotation)
        {
            Vector3 euler = rotation.eulerAngles;
            PlayerLook.SetRotation(euler.x, euler.y);
        }
    }
}