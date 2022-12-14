using Fungus;
using Player;
using Player.Components;
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

        private SceneLoadingService _sceneLoadingService;
        private FungusService _fungusService;
        private PlayerService _playerService;
        private Transform _transform;
        private float _startYPosition;

        public override void TrySetFloorSteps(AudioClip[] clips)
        {
            PlayerWalksteps.SetFloorSteps(clips);
        }

        public void Awake()
        {
            _startYPosition = transform.position.y;
            _transform = transform;
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            _fungusService = ServiceLocator.Get<FungusService>();
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnEnable()
        {
            _sceneLoadingService.SceneLoader.OnStartLoad += OnLoadingStart;
            _fungusService.OnBlockStart += OnBlockStart;
            _fungusService.OnBlockEnd += OnBlockEnd;
        }

        private void OnDisable()
        {
            _sceneLoadingService.SceneLoader.OnStartLoad -= OnLoadingStart;
            _fungusService.OnBlockStart -= OnBlockStart;
            _fungusService.OnBlockEnd -= OnBlockEnd;
        }

        private void OnLoadingStart() => SetActive(false);

        private void OnBlockStart(Block block) => SetActive(false);

        private void OnBlockEnd(Block block) => SetActive(true);

        public void Update()
        {
            playerInputFPS.UpdateInvoke();
            PlayerLook.UpdateInvoke();
            PlayerInteract.UpdateInvoke();

            if (_transform.position.y < _startYPosition - 50f) {
                SetPosition(_playerService.DefaultPlayerSpawn.transform.position);
                SetRotation(_playerService.DefaultPlayerSpawn.transform.rotation);
            }
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