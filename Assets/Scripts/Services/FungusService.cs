﻿using System;
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
        public event BlockSignals.BlockStartHandler OnBlockStart;
        public event BlockSignals.BlockEndHandler OnBlockEnd;
        
        private bool _isDialogue = false;

        public bool IsDialogue => _isDialogue;

        public void AwakeService()
        {
            _gameService = ServiceLocator.Get<GameService>();

            BlockSignals.OnBlockStart += OnBlockStart;
            BlockSignals.OnBlockStart += OnBlockStartAction;
            BlockSignals.OnBlockEnd += OnBlockEnd;
            BlockSignals.OnBlockEnd += OnBlockEndAction;
            
            SayDialog.Construct(_factoryService);
        }
        
        public void TerminateService()
        {
            BlockSignals.OnBlockStart -= OnBlockStart;
            BlockSignals.OnBlockEnd -= OnBlockEnd;
        }

        private void OnBlockStartAction(Block block)
        {
            _isDialogue = true;
            _gameService.SetDialogueState(true);
        }

        private void OnBlockEndAction(Block block)
        {
            _isDialogue = false;
            _gameService.SetDialogueState(false);
        }
    }
}