using DG.Tweening;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class MusicPlayerService : Service, IAwakeService
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private float _duration;
        
        public AudioSource MusicSource => _musicSource;
        public float Duration => _duration;
        
        private float _defaultVolume;
        private SceneLoadingService _sceneService;

        public void AwakeService()
        {
            _defaultVolume = _musicSource.volume;
            _sceneService = ServiceLocator.Get<SceneLoadingService>();
        }
        
        public void Show(float duration)
        {
            DOTween.To(() => _musicSource.volume, _ => _musicSource.volume = _, _defaultVolume, duration);
        }

        public void Hide(float duration)
        {
            DOTween.To(() => _musicSource.volume, _ => _musicSource.volume = _, 0, duration);
        }

        public void Show()
        {
            Show(_duration);
        }

        public void Hide()
        {
            Hide(_duration);
        }
    }
}