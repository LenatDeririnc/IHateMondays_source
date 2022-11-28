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

        public void AwakeService()
        {
            _sceneLoader.Construct(_loadingCurtain);
        }

        public void LoadScene(PlayerSpawnPointInfo playerSpawn)
        {
            _sceneLoader.LoadScene(playerSpawn.Scene);
            _currentSpawnInfo = playerSpawn;
        }

        public void LoadScene(SceneLink scene, CurtainType curtainType = CurtainType.AlphaTransition)
        {
            _sceneLoader.LoadScene(scene, curtainType);
            _currentSpawnInfo = null;
        }
    }
}