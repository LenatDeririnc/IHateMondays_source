using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class GameService : Service, IAwakeService, ITerminateService
    {
        private InputBridgeService _inputBridgeService;
        [SerializeField] private bool _cursorEnabledInScene = true;
        
        private bool _isPlayingDialogue;
        public bool IsPlayingDialogue => _isPlayingDialogue;

        public void AwakeService()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _inputBridgeService.SetCursorLocked(!_cursorEnabledInScene);
        }

        public void SetDialogueState(bool value)
        {
            _isPlayingDialogue = value;
        }

        public void TerminateService()
        {
            _inputBridgeService.SetCursorLocked(false);
        }
    }
}