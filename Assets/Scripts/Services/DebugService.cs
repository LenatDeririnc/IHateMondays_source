using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace Services
{
    public class DebugService : Service, IAwakeService, IUpdateService
    {
        [SerializeField] private int clampedFps;
        [SerializeField] private Canvas loadSceneCanvas;
        [SerializeField] private SceneLink[] scenesForLoad;
        [SerializeField] private bool isLoadingCanvasActive = false;
        
        private int _previousClampedFps;

        private UIFactory _uiFactory;
        private SceneLoadingService _sceneLoadingService;

        public void AwakeService()
        {
            if (!Application.isEditor)
                return;
            
            _uiFactory = ServiceLocator.Get<UIFactory>();
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();

            foreach (SceneLink scene in scenesForLoad) {
                var button = _uiFactory.CreateButton(scene.sceneName, Vector3.zero,
                    () => _sceneLoadingService.LoadScene(scene),
                    loadSceneCanvas.transform);
            }
            
            loadSceneCanvas.gameObject.SetActive(isLoadingCanvasActive);
        }

        public void UpdateService()
        {
            if (_previousClampedFps != clampedFps)
            {
                Application.targetFrameRate = clampedFps;
                _previousClampedFps = clampedFps;
            }
            
            if (!Application.isEditor)
                return;

            if (Input.GetKeyDown(KeyCode.F1)) {
                isLoadingCanvasActive = !isLoadingCanvasActive;
                loadSceneCanvas.gameObject.SetActive(isLoadingCanvasActive);
            }
        }
    }
}