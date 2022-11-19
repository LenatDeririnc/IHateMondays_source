using Plugins.ServiceLocator;
using Services;
using Tools;
using UnityEngine;

namespace Scenes
{
    public class RespawnPlayerTrigger : MonoBehaviour
    {
        [SerializeField] private Trigger _trigger;
        private HorrorService _respawnService;
        private PlayerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _respawnService = ServiceLocator.Get<HorrorService>();
            _trigger.OnEnter += OnEnter;
        }

        private void OnEnter(Collider obj)
        {
            if (_playerService.Player.Collider != obj)
                return;

            _respawnService.Respawn();
        }
    }
}