using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class GameService : Service, IAwakeService, ITerminateService
    {
        private InputBridgeService _inputBridgeService;
        [SerializeField] private bool _cursorEnabledInScene = true;

        public void AwakeService()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _inputBridgeService.SetCursorLocked(!_cursorEnabledInScene);
        }

        public void TerminateService()
        {
            _inputBridgeService.SetCursorLocked(false);
        }
    }
}