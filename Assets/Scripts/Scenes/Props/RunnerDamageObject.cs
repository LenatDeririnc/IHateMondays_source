using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class RunnerDamageObject : MonoBehaviour
    {
        private RunnerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<RunnerService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != _playerService.RunnerController.Collider)
                return;
            
            _playerService.ReceiveDamage();
        }
    }
}