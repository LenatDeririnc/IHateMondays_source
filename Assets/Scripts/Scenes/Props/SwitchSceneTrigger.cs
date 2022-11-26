using Plugins.ServiceLocator;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class SwitchSceneTrigger : MonoBehaviour
    {
        [SerializeField] private SceneLink _switchScene;
        
        private PlayerService _playerService;
        private SceneLoadingService _sceneLoadingService;

        private void Awake()
        {
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;
            
            _sceneLoadingService.LoadScene(_switchScene);
        }
    }
}