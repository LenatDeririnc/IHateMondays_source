using Fungus;
using Plugins.ServiceLocator;
using SceneManager;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class SwitchSceneTrigger : MonoBehaviour
    {
        [SerializeField] private SceneLink _switchScene;
        [SerializeField] private Flowchart _flowchart;
        [SerializeField] private string _blockName;
        
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

            _flowchart.ExecuteBlock(_blockName);
        }

        public void LoadScene()
        {
            _sceneLoadingService.LoadScene(_switchScene, CurtainType.None);
        }

        public void DisablePlayer()
        {
            ServiceLocator.Get<PlayerService>().Player.gameObject.SetActive(false);
        }
    }
}