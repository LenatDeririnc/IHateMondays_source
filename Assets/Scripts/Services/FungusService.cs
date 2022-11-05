using Fungus;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;

namespace Services
{
    public class FungusService : Service, IAwakeService
    {
        private GameService _gameService;

        public void AwakeService()
        {
            _gameService = ServiceLocator.Get<GameService>();
            
            BlockSignals.OnBlockStart += OnBlockStart;
            BlockSignals.OnBlockEnd += OnBlockEnd;
        }

        private void OnBlockStart(Block block)
        {
            _gameService.SetDialogueState(true);
        }

        private void OnBlockEnd(Block block)
        {
            _gameService.SetDialogueState(false);
        }
    }
}