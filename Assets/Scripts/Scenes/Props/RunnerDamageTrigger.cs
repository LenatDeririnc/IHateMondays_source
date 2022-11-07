using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class RunnerDamageTrigger : MonoBehaviour
    {
        private PlayerService _playerService;
        
        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == _playerService.Player.Collider) {
                _playerService.Player.ReceiveDamage();
            }
        }
    }
}