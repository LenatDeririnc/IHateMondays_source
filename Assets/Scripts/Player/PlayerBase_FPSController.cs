using Player;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using TransformTools;
using UnityOverrides;

namespace Characters.Components
{
    public class PlayerBase_FPSController : PlayerBase, ISelfDeps
    {
        public PlayerMovementBase PlayerMovement;
        public PlayerGravityBase PlayerGravityBase;
        public PlayerInput PlayerInput;
        public PlayerLook PlayerLook;
        public PlayerWalksteps PlayerWalksteps;
        public PlayerInteract PlayerInteract;
        public CharacterControllerDecorator CharacterControllerDecorator;
        
        private SceneLoadingService SceneLoadingService;

        public void Awake()
        {
            SceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            SceneLoadingService.OnLoadingStart += OnLoading;
        }
        
        public void Update()
        {
            PlayerWalksteps.UpdateInvoke();
            PlayerInput.UpdateInvoke();
            PlayerMovement.UpdateInvoke();
            PlayerLook.UpdateInvoke();
            PlayerGravityBase.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
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
            PlayerInput = GetComponent<PlayerInput>();
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