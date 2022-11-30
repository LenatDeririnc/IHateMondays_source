using Plugins.ServiceLocator;
using Scenes.Props;
using Services;
using UnityEngine;

namespace Scenes
{
    public class SwitchLightTrigger : MonoBehaviour
    {
        [SerializeField] private LightLamp _originLamp;
        private PlayerService _playerService;
        private bool _isEntered = false;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        public void Reset()
        {
            _isEntered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != _playerService.Player.Collider)
                return;
                
            if (_isEntered)
                return;
            
            if (_originLamp.OnEnterTrigger != null)
                _originLamp.OnEnterTrigger.SetActive(true);

            if (_originLamp.HorrorPathTriggerToTurnOff != null)
                _originLamp.HorrorPathTriggerToTurnOff.SetActive(false);
            
            if (_originLamp.LampToTurnOn != null)
                _originLamp.LampToTurnOn.SwitchOn();
            
            if (_originLamp.LampToTurnOff != null)
                _originLamp.LampToTurnOff.SwitchOff();
            
            _isEntered = true;
        }
    }
}