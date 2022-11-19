using Plugins.ServiceLocator;
using Scenes.Props;
using Services;
using UnityEngine;

namespace Scenes
{
    public class EnterLightTriggerZone : MonoBehaviour
    {
        [SerializeField] private LightLamp _lampToTurnOn;
        private bool _isEntered = false;
        private PlayerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other != _playerService.Player.Collider)
                return;
                
            if (_isEntered)
                return;
            
            if (_lampToTurnOn != null)
                _lampToTurnOn.Switch(1);

            _isEntered = true;
        }

        public void Reset()
        {
            _isEntered = false;
        }
    }
}