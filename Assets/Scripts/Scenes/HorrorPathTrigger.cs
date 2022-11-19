using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes
{
    public class HorrorPathTrigger : MonoBehaviour
    {
        private PlayerService _playerService;
        private HorrorService _horrorService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _horrorService = ServiceLocator.Get<HorrorService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;

            _horrorService.HorrorPath.CurrentPathEntered.Add(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;
            
            _horrorService.HorrorPath.CurrentPathEntered.Remove(this);
            _horrorService.HorrorPath.CheckReset();
        }

        public void SetActive(bool b)
        {
            if (!b) {
                if (_horrorService.HorrorPath.CurrentPathEntered.Contains(this))
                    _horrorService.HorrorPath.CurrentPathEntered.Remove(this);
            }
            gameObject.SetActive(b);
        }
    }
}