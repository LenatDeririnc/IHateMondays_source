using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using Tools;
using UnityEngine;
using UnityEngine.Rendering;
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
        [SerializeField] private float _damageSwitchInterval = 0.5f;

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

        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static Color DamagedColor => new Color(1,1,1,0.5f);

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

        private void SetMaterialDamaged()
        {
            foreach (var rend in _runnerController.Renderers) {
                rend.shadowCastingMode = ShadowCastingMode.Off;
                StandardShaderUtils.ChangeRenderMode(rend.material, StandardShaderUtils.BlendMode.Fade);
                rend.material.SetColor(ColorProperty, DamagedColor);
            }
        }

        private void SetMaterialNormal()
        {
            foreach (var rend in _runnerController.Renderers) {
                rend.shadowCastingMode = ShadowCastingMode.On;
                StandardShaderUtils.ChangeRenderMode(rend.material, StandardShaderUtils.BlendMode.Opaque);
                rend.material.SetColor(ColorProperty, Color.white);
            }
        }

        private void SetColor(Color color)
        {
            foreach (var rend in _runnerController.Renderers) {
                rend.material.SetColor(ColorProperty, color);
            }
        }
        
        private Sequence _sequence;
        private void StartBlink()
        {
            _sequence = DOTween.Sequence()
                .AppendInterval(_damageSwitchInterval)
                .AppendCallback(() => SetColor(Color.clear))
                .AppendInterval(_damageSwitchInterval)
                .AppendCallback(() => SetColor(DamagedColor))
                .SetLoops(-1);
        }

        private void StopBlink()
        {
            _sequence.Kill();
        }

        private IEnumerator Damage()
        {
            SetSlownDownSpeed();
            SetMaterialDamaged();
            StartBlink();
            
            yield return new WaitForSeconds(DamageSeconds);
            
            StopBlink();
            SetMaterialNormal();
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