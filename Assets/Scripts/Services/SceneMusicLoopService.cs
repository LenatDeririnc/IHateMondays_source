using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class SceneMusicLoopService : Service, IAwakeService
    {
        [SerializeField] private AudioClip _loop;
        private MusicPlayerService _musicService;

        public void AwakeService()
        {
            _musicService = ServiceLocator.Get<MusicPlayerService>();
            _musicService.PlayLoop(_loop);
        }
    }
}