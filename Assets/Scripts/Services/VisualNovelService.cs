using Fungus;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace Services
{
    public class VisualNovelService : Service, IAwakeService
    {
        [SerializeField] private Stage _stage;
        [SerializeField] private Flowchart _flowchart;
        [SerializeField] private string _blockNameStart = "Start";
        [SerializeField] private SceneLink _endScene;
        private SceneLoadingService _sceneLoader;
        public Stage Stage => _stage;
        public void AwakeService()
        {
            _sceneLoader = ServiceLocator.Get<SceneLoadingService>();
            _flowchart.ExecuteBlock(_blockNameStart);
        }

        public void NextScene()
        {
            _sceneLoader.LoadScene(_endScene);
        }
    }
}