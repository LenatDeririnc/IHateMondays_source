using System;
using Player;
using Player.Custom;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using TransformTools;
using UnityOverrides;

namespace Characters.Components
{
    public class FPSController : PlayerBase, ISelfDeps
    {
        public PlayerMovementBase PlayerMovement;
        public PlayerGravityBase PlayerGravityBase;
        public PlayerInput_FPS playerInputFPS;
        public PlayerLook PlayerLook;
        public PlayerWalksteps PlayerWalksteps;
        public PlayerInteract PlayerInteract;
        public CharacterControllerDecorator CharacterControllerDecorator;
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
            PlayerMovement.UpdateInvoke();
            PlayerLook.UpdateInvoke();
            PlayerGravityBase.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
            FlashlightMovement.UpdateInvoke();
        }

        public void FixedUpdate()
        {
            PlayerMovement.FixedUpdateInvoke();
            PlayerGravityBase.FixedUpdateInvoke();
        }

        public void SetupDeps()
        {
            CharacterControllerDecorator = GetComponent<CharacterControllerDecorator>();
            PlayerMovement = GetComponent<PlayerMovementBase>();
            PlayerGravityBase = GetComponent<PlayerGravityBase>();
            playerInputFPS = GetComponent<PlayerInput_FPS>();
            PlayerLook = GetComponent<PlayerLook>();
            PlayerWalksteps = GetComponent<PlayerWalksteps>();
        }

        public void SetTransformData(TransformData spawnPoint)
        {
            CharacterControllerDecorator.SetPosition(spawnPoint.position);
            CharacterControllerDecorator.SetRotation(spawnPoint.rotation);
        }

        public void OnLoading()
        {
            PlayerMovement.enabled = false;
            PlayerInteract.enabled = false;
            PlayerLook.enabled = false;
        }
    }
}