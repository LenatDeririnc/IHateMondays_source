using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Audio
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private AudioSource _source;
        private PlayerService _playerService;

        private bool isEntered = false;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;
            
            if (isEntered)
                return;

            isEntered = true;
            _source.PlayOneShot(_clip);
        }
    }
}