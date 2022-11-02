using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;

namespace Services
{
    public class GameService : Service, IAwakeService, IUpdateService
    {
        private InputBridgeService _inputBridgeService;
        
        public bool IsPaused { get; set; }
        public bool CanPressPause { get; set; } = true;
        
        public void AwakeService()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _inputBridgeService.SetCursorLocked(true);
        }

        public void UpdateService()
        {
            if (_inputBridgeService.IsPauseButtonDown && CanPressPause) {
                IsPaused = !IsPaused;
            }
        }
    }
}