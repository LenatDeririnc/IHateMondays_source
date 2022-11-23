using System.Collections;
using DG.Tweening;
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
        [SerializeField][Range(0,1)] private float _speedPercentOnPlayTriedAnimation = 0.3f;

        public float MoveDistance => _moveDistance;

        public SplineContainer Spline => _spline;

        public float CurrentSpeed => _currentSpeed;
        public float DamageSeconds => _damageSeconds;

        private float _currentSpeed;
        private PlayerService _playerService;
        private RunnerController _runnerController;
        private Coroutine _damageCoroutine;
        private static readonly int InjuredProperty = Animator.StringToHash("Injured");
        private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int Tried = Animator.StringToHash("Tried");

        public RunnerController RunnerController => _runnerController;
        public bool IsEnding = false;
        private bool _isPlayingFail = false;

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
            if (_isPlayingFail)
                return;
            
            _currentSpeed = value;
            _runnerController.Animator.SetFloat(MoveSpeed, _currentSpeed / _defaultSpeed);

            if (IsEnding && !_isPlayingFail && _currentSpeed / _defaultSpeed < _speedPercentOnPlayTriedAnimation) {
                PlayFailAnimation();
            }
        }

        public void PlayFailAnimation()
        {
            DOTween.To(
                    () => _currentSpeed, 
                    _ => _currentSpeed = _, 
                    0, 
                    0.5f);
            
            _isPlayingFail = true;
            _runnerController.Animator.applyRootMotion = true;
            _runnerController.Animator.SetTrigger(Tried);
        }
    }
}