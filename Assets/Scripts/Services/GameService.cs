using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine.Device;

namespace Services
{
    public class GameService : Service, IAwakeService, IUpdateService
    {
        private InputBridgeService _inputBridgeService;

        private bool _isPaused;
        private bool _isPlayingDialogue;
        public bool IsPaused => _isPaused || _isPlayingDialogue;
        public bool CanPressPause { get; set; } = true;
        
        public void AwakeService()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _inputBridgeService.SetCursorLocked(true);
        }

        public void SetDialogueState(bool value)
        {
            _isPlayingDialogue = value;
        }

        public void UpdateService()
        {
            if (!Application.isEditor)
                return;
            
            if (_inputBridgeService.IsPauseButtonDown && CanPressPause) {
                _isPaused = !_isPaused;
                _inputBridgeService.SetCursorLocked(!_isPaused);
            }
        }
    }
}