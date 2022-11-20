using Fungus;
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
        
        private SceneLoadingService _sceneLoadingService;
        private FungusService _fungusService;

        public void Awake()
        {
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            _fungusService = ServiceLocator.Get<FungusService>();
            
            _sceneLoadingService.OnLoadingStart += OnLoadingStart;
            _fungusService.OnBlockStart += OnBlockStart;
            _fungusService.OnBlockEnd += OnBlockEnd;
        }

        private void OnDestroy()
        {
            _sceneLoadingService.OnLoadingStart -= OnLoadingStart;
            _fungusService.OnBlockStart -= OnBlockStart;
            _fungusService.OnBlockEnd -= OnBlockEnd;
        }

        private void OnBlockStart(Block block)
        {
            SetActive(false);
        }

        private void OnBlockEnd(Block block)
        {
            SetActive(true);
        }

        private void OnLoadingStart()
        {
            SetActive(false);
        }

        public void Update()
        {
            PlayerWalksteps.UpdateInvoke();
            playerInputFPS.UpdateInvoke();
            PlayerLook.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
            FlashlightMovement?.UpdateInvoke();
        }

        public void SetActive(bool value)
        {
            PlayerMovementBase.enabled = value;
            PlayerInteract.enabled = value;
            PlayerLook.enabled = value;
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