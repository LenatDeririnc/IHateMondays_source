using System;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using SceneManager;
using SceneManager.ScriptableObjects;
using Scenes.PlayerSpawner;
using UnityEngine;

namespace Services
{
    public class SceneLoadingService : Service, IAwakeService
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private LoadingCurtainManager _loadingCurtain;
        
        private PlayerSpawnPointInfo _currentSpawnInfo;
        
        public SceneLoader SceneLoader => _sceneLoader;
        public LoadingCurtainManager LoadingCurtain => _loadingCurtain;
        public PlayerSpawnPointInfo CurrentSpawnInfo => _currentSpawnInfo;

        public Action OnLoadingStart;

        public void AwakeService()
        {
            _loadingCurtain.Construct();
            _sceneLoader.Construct(_loadingCurtain);
        }

        public void LoadScene(PlayerSpawnPointInfo playerSpawn)
        {
            OnLoadingStart?.Invoke();
            _sceneLoader.LoadScene(playerSpawn.Scene);
            _currentSpawnInfo = playerSpawn;
        }

        public void LoadScene(SceneLink scene)
        {
            OnLoadingStart?.Invoke();
            _sceneLoader.LoadScene(scene);
            _currentSpawnInfo = null;
        }
    }
}