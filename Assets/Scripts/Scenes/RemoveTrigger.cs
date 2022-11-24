using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes
{
    public class RemoveTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objectsToRemove;
        private PlayerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other != _playerService.Player.Collider)
                return;
            
            foreach (var obj in _objectsToRemove) {
                obj.SetActive(false);
            }
        }
    }
}