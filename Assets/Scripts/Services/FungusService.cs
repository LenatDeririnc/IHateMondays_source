using Fungus;
using Fungus.Services;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class FungusService : Service, IAwakeService, ITerminateService
    {
        [SerializeField] private FungusFactoryService _factoryService;
        
        private GameService _gameService;
        public void AwakeService()
        {
            _gameService = ServiceLocator.Get<GameService>();
            
            BlockSignals.OnBlockStart += OnBlockStart;
            BlockSignals.OnBlockEnd += OnBlockEnd;
            
            SayDialog.Construct(_factoryService);
        }
        
        public void TerminateService()
        {
            BlockSignals.OnBlockStart -= OnBlockStart;
            BlockSignals.OnBlockEnd -= OnBlockEnd;
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