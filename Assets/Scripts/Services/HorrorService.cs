using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using Scenes;
using UnityEngine;

namespace Services
{
    public class HorrorService : Service, IAwakeService
    {
        [SerializeField] private HorrorPath _horrorPath;
        [SerializeField] private Transform _positionToRespawn;
        [SerializeField] private GameObject _keysCanvas;
        private PlayerService _playerService;
        private CameraService _cameraService;

        public HorrorPath HorrorPath => _horrorPath;

        public void AwakeService()
        {
            _cameraService = ServiceLocator.Get<CameraService>();
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        public void ResetAll()
        {
            _horrorPath.Reset();
            _playerService.Player.SetPositionAndRotation(_positionToRespawn);
        }

        public void Respawn()
        {
            _playerService.Player.SetPositionAndRotation(_positionToRespawn);
        }

        public void PlayKeysCanvas()
        {
            _cameraService.UICamera.gameObject.SetActive(true);
            _keysCanvas.SetActive(true);
        }
        
        public void StopKeysCanvas()
        {
            _keysCanvas.SetActive(false);
            _cameraService.UICamera.gameObject.SetActive(false);
        }
    }
}