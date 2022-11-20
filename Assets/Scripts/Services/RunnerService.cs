using System.Collections;
using Player;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;
using UnityEngine.Splines;

namespace Services
{
    public class RunnerService : Service, IAwakeService
    {
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private float _slowDownSpeed;
        [SerializeField] private float _damageSeconds = 5f;
        [SerializeField] private SplineContainer _spline;
        [SerializeField] private float _moveDistance = 10f;

        public float MoveDistance => _moveDistance;

        public SplineContainer Spline => _spline;

        public float CurrentSpeed => _currentSpeed;
        public float DamageSeconds => _damageSeconds;

        private float _currentSpeed;
        private PlayerService _playerService;
        private RunnerController _runnerController;
        private Coroutine _damageCoroutine;
        private static readonly int InjuredProperty = Animator.StringToHash("Injured");

        public RunnerController RunnerController => _runnerController;

        public void AwakeService()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _runnerController = (RunnerController)_playerService.Player;
            SetDefaultSpeed();
        }

        public void SetDefaultSpeed()
        {
            _currentSpeed = _defaultSpeed;
            _runnerController.Animator.SetBool(InjuredProperty, false);
        }

        public void SetSlownDownSpeed()
        {
            _currentSpeed = _slowDownSpeed;
            _runnerController.Animator.SetBool(InjuredProperty, true);
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

        public void SetCurrentSpeed(float value)
        {
            _currentSpeed = value;
        }
    }
}