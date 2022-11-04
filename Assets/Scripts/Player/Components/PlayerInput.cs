﻿using System;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInput : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerMovementBase _playerMovement;
        [SerializeField] private PlayerGravityBase _playerGravityBase;
        [SerializeField] private PlayerLook _playerLook;
        
        private InputBridgeService InputBridgeService;
        private GameService GameService;

        private void Awake()
        {
            InputBridgeService = ServiceLocator.Get<InputBridgeService>();
            GameService = ServiceLocator.Get<GameService>();
        }

        public void SetupDeps()
        {
            _playerMovement = GetComponent<PlayerMovementBase>();
            _playerGravityBase = GetComponent<PlayerGravityBase>();
            _playerLook = GetComponent<PlayerLook>();
        }

        private void JumpInputUpdate()
        {
            if (InputBridgeService.IsJumpUp)
            {
                _playerGravityBase.SetJumpCommand(false);
                _playerGravityBase.CutoffJump();
            }

            if (InputBridgeService.IsJumpDown)
            {
                _playerGravityBase.SetJumpCommand(true);
            }
        }

        private void MovementInputUpdate()
        {
            var movement = InputBridgeService.Movement;
            _playerMovement.SetMovement(new Vector3(movement.x, 0, movement.y));
        }

        private void RotationInputUpdate()
        {
            _playerLook.SetRotateDelta(new Vector3(InputBridgeService.Look.x, InputBridgeService.Look.y, 0));
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (!GameService.IsPaused)
            {
                MovementInputUpdate();
                RotationInputUpdate();
                JumpInputUpdate();
            }
            else
            {
                PausedUpdate();
            }
        }

        public void PausedUpdate()
        {
            _playerMovement.SetMovement(Vector3.zero);
            _playerLook.SetRotateDelta(Vector3.zero);
            _playerGravityBase.SetJumpCommand(false);
            _playerGravityBase.CutoffJump();
        }
    }
}