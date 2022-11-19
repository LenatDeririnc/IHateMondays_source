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
        private PlayerService _playerService;

        public HorrorPath HorrorPath => _horrorPath;

        public void AwakeService()
        {
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
    }
}