using Plugins.ServiceLocator;
using Scenes.PlayerSpawner;
using Services;
using UnityEngine;

namespace Props
{
    public class LocationDoor : MonoBehaviour, IInteractable
    {
        public PlayerSpawnPointInfo SpawnRelocation;
        private SceneLoadingService _loader;
        
        public void Awake()
        {
            _loader = ServiceLocator.Get<SceneLoadingService>();
        }
        
        public void Interact()
        {
            _loader.LoadScene(SpawnRelocation);
        }

        public bool IsAvailableToInteract()
        {
            return true;
        }
    }
}
