using Fungus;
using Player;
using Player.Components;
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

        private SceneLoadingService _sceneLoadingService;
        private FungusService _fungusService;

        public override void TrySetFloorSteps(AudioClip[] clips)
        {
            PlayerWalksteps.SetFloorSteps(clips);
        }

        public void Awake()
        {
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            _fungusService = ServiceLocator.Get<FungusService>();

            _sceneLoadingService.SceneLoader.OnStartLoad += OnLoadingStart;
            _fungusService.OnBlockStart += OnBlockStart;
            _fungusService.OnBlockEnd += OnBlockEnd;
        }

        private void OnDestroy()
        {
            _sceneLoadingService.SceneLoader.OnStartLoad -= OnLoadingStart;
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
            playerInputFPS.UpdateInvoke();
            PlayerLook.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
        }

        public void SetActive(bool value)
        {
            try
            {
                PlayerMovementBase.enabled = value;
                PlayerInteract.enabled = value;
                PlayerLook.enabled = value;
            }
            catch
            {
                Debug.Log("FPSController crash from set active - null ref components");
            }
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