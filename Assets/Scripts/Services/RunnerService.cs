﻿using System.Collections;
using Cinemachine;
using Player;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class RunnerService : Service, IAwakeService
    {
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private float _slowDownSpeed;
        [SerializeField] private float _damageSeconds = 5f;
        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtual;
        public float CurrentSpeed => _currentSpeed;
        public float DamageSeconds => _damageSeconds;

        private float _currentSpeed;
        private PlayerService _playerService;
        private RunnerController _runnerController;
        private Coroutine _damageCoroutine;

        public RunnerController RunnerController => _runnerController;

        public void AwakeService()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _runnerController = (RunnerController)_playerService.Player;
            _cinemachineVirtual.LookAt = _runnerController.CameraView;
            _cinemachineVirtual.Follow = _runnerController.CameraPosition;
            SetDefaultSpeed();
        }

        public void SetDefaultSpeed()
        {
            _currentSpeed = _defaultSpeed;
        }

        public void SetSlownDownSpeed()
        {
            _currentSpeed = _slowDownSpeed;
        }

        public void ReceiveDamage()
        {
            if (_damageCoroutine != null) {
                StopCoroutine(_damageCoroutine);
            }
            _damageCoroutine = StartCoroutine(Damage());
        }

        private IEnumerator Damage()
        {
            SetSlownDownSpeed();
            yield return new WaitForSeconds(DamageSeconds);
            SetDefaultSpeed();
        }
    }
}