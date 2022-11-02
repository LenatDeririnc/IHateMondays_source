using MainMenu.SettingsMenu;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace Services
{
    public class MainMenuSceneService : Service, IAwakeService
    {
        [SerializeField] private SetSensitivity _setSensitivityOption;
        [SerializeField] private SceneLink _nextScene;
        private SceneLoadingService _sceneService;
        private InputBridgeService _inputBridgeService;

        public void AwakeService()
        {
            _sceneService = ServiceLocator.Get<SceneLoadingService>();
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            
            _inputBridgeService.InitSensitivityOption(_setSensitivityOption);
        }

        public void OnStartGame()
        {
            _sceneService.LoadScene(_nextScene);
        }
    }
}