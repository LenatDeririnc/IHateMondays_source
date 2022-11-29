using System;
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
        public event BlockSignals.BlockStartHandler OnBlockStart;
        public event BlockSignals.BlockEndHandler OnBlockEnd;

        public event Action OnMenuStart;
        public event Action OnMenuEnd;
        
        private bool _isDialogue = false;
        private bool _isMenu = false;

        public bool IsDialogue => _isDialogue || _isMenu;

        public void AwakeService()
        {
            BlockSignals.OnBlockStart += OnBlockStartAction;
            BlockSignals.OnBlockEnd += OnBlockEndAction;
            
            BlockSignals.OnBlockStart += OnBlockStart;
            BlockSignals.OnBlockEnd += OnBlockEnd;

            MenuDialog.OnMenuDialogEnabled += OnMenuStart;
            MenuDialog.OnMenuDialogDisabled += OnMenuEnd;

            MenuDialog.OnMenuDialogEnabled += OnMenuDialogueStartAction;
            MenuDialog.OnMenuDialogDisabled += OnMenuDialogueEndAction;

            SayDialog.Construct(_factoryService);
        }

        public void TerminateService()
        {
            BlockSignals.OnBlockStart -= OnBlockStartAction;
            BlockSignals.OnBlockEnd -= OnBlockEndAction;
            
            BlockSignals.OnBlockStart -= OnBlockStart;
            BlockSignals.OnBlockEnd -= OnBlockEnd;
            
            MenuDialog.OnMenuDialogEnabled -= OnMenuStart;
            MenuDialog.OnMenuDialogDisabled -= OnMenuEnd;
            
            MenuDialog.OnMenuDialogEnabled -= OnMenuDialogueStartAction;
            MenuDialog.OnMenuDialogDisabled -= OnMenuDialogueEndAction;
        }

        private void OnMenuDialogueStartAction()
        {
            _isMenu = true;
        }

        private void OnMenuDialogueEndAction()
        {
            _isMenu = false;
        }

        private void OnBlockStartAction(Block block)
        {
            _isDialogue = true;
        }

        private void OnBlockEndAction(Block block)
        {
            _isDialogue = false;
        }
    }
}