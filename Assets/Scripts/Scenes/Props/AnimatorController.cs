using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Props
{
    public class AnimatorController : MonoBehaviour
    {
        public Animator Animator;
        private GameService _gameService;

        private void Awake()
        {
            _gameService = ServiceLocator.Get<GameService>();
        }

        private void Update()
        {
            Animator.speed = _gameService.IsPlayingDialogue ? 0 : 1;
        }
    }
}