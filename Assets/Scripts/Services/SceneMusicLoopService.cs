using Plugins.ServiceLocator;
using UnityEngine;

namespace Services
{
    public class SceneMusicLoopService : Service
    {
        [SerializeField] private AudioClip _intro;
        [SerializeField] private AudioClip _loop;

        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _oldMusicFadeOutDuration = 0.5f;
        
        private AudioService _musicService;

        public void Start()
        {
            _musicService = ServiceLocator.Get<AudioService>();
            _musicService.PlayBackgroundMusic(_intro, _loop, _oldMusicFadeOutDuration, _fadeInDuration);
        }
    }
}