using System;
using Plugins.ServiceLocator;
using Props;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class SwitchSceneInteractable: MonoBehaviour, IInteractable
    {
        [SerializeField] private SceneLink _switchScene;
        private SceneLoadingService _sceneLoadingService;

        private void Awake()
        {
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
        }

        public void Interact()
        {
            _sceneLoadingService.LoadScene(_switchScene);
        }

        public bool IsAvailableToInteract()
        {
            return true;
        }
    }
}