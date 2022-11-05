using Characters.Components;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;

namespace Player
{
    public class ZeldaController : PlayerBase, ISelfDeps
    {
        public PlayerMovementBase PlayerMovement;
        public PlayerInput_Zelda PlayerInput;
        public PlayerInteract PlayerInteract;

        private SceneLoadingService SceneLoadingService;

        private void Awake()
        {
            SceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            SceneLoadingService.OnLoadingStart += OnLoading;
        }

        public void OnLoading()
        {
            PlayerMovement.enabled = false;
        }

        public void Update()
        {
            PlayerInput.UpdateInvoke();
            PlayerMovement.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
        }
        
        public void FixedUpdate()
        {
            PlayerMovement.FixedUpdateInvoke();
        }
        
        public void SetupDeps()
        {
            PlayerMovement = GetComponent<PlayerMovementBase>();
            PlayerInput = GetComponent<PlayerInput_Zelda>();
        }
    }
}