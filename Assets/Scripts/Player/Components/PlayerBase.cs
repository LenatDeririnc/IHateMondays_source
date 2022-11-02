using Plugins.ServiceLocator;
using Services;
using TransformTools;
using UnityEngine;
using UnityOverrides;

namespace Characters.Components
{
    public class PlayerBase : Character
    {
        public InputBridgeService InputBridgeService { get; private set; }
        public AudioSourcesService AudioSourcesService { get; private set; }

        public GameService GameService { get; private set; }
        public SceneLoadingService SceneLoadingService { get; private set; }

        public CharacterControllerDecorator PlayerController;
        public PlayerMovementBase PlayerMovement;
        public PlayerGravityBase PlayerGravityBase;
        public PlayerInput PlayerInput;
        public PlayerLook PlayerLook;
        public PlayerSound PlayerSound;
        public Transform PlayerTransform;
        public Transform CameraTransform;
        public PlayerWalksteps PlayerWalksteps;
        public PlayerInteract PlayerInteract;

        public void Awake()
        {
            InputBridgeService = ServiceLocator.Get<InputBridgeService>();
            AudioSourcesService = ServiceLocator.Get<AudioSourcesService>();
            GameService = ServiceLocator.Get<GameService>();
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

        public override void SetupDeps()
        {
            base.SetupDeps();
            PlayerMovement = GetComponent<PlayerMovementBase>();
            PlayerGravityBase = GetComponent<PlayerGravityBase>();
            PlayerInput = GetComponent<PlayerInput>();
            PlayerLook = GetComponent<PlayerLook>();
            PlayerSound = GetComponent<PlayerSound>();

            PlayerTransform = transform;
            CameraTransform = GetComponentInChildren<Camera>().transform;
            
            PlayerWalksteps = GetComponent<PlayerWalksteps>();
        }

        public void SetTransformData(TransformData spawnPoint)
        {
            CharacterController.SetPosition(spawnPoint.position);
            CharacterController.SetRotation(spawnPoint.rotation);
        }

        public void OnLoading()
        {
            PlayerMovement.enabled = false;
            PlayerInteract.enabled = false;
            PlayerLook.enabled = false;
        }
    }
}