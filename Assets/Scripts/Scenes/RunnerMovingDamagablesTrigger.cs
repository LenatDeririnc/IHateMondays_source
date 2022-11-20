using Plugins.ServiceLocator;
using Scenes.Props;
using Services;
using UnityEngine;

namespace Scenes
{
    public class RunnerMovingDamagablesTrigger : MonoBehaviour
    {
        [SerializeField] private RunnerDamageMovingObject[] _movingObjects;
        private PlayerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != _playerService.Player.Collider)
                return;

            foreach (var obj in _movingObjects) {
                obj.StartMove();
            }
        }
    }
}