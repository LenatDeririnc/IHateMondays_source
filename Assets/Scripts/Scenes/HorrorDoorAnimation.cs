using UnityEngine;
using UnityEngine.Playables;

namespace Scenes
{
    public class HorrorDoorAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _interactableGameObject;
        [SerializeField] private PlayableDirector _director;
        
        public void StartAnimation()
        {
            _director.Play();
            _interactableGameObject.SetActive(false);
        }
    }
}