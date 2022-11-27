using DG.Tweening;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using UnityEngine.Splines;
using UnityOverrides;

namespace Player
{
    public class RunnerController : PlayerBase
    {
        [SerializeField] private RunnerControllerDecorator _characterTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private GameObject _renderersGameObject;
        [SerializeField] private ParticleSystem _stepParticles;

        public GameObject RenderersGameObject => _renderersGameObject;

        public Renderer[] Renderers => _renderers;

        public Animator Animator => _animator;
        public bool CanMove { get; set; } = true;

        private int _currentRoadIndex = 0;
        private float _splinePosition = 0f;

        private bool _isLineSwitchInProgress;
        
        private InputBridgeService _inputBridgeService;
        private RunnerService _runnerService;
        
        private Coroutine _damageCoroutine;
        private static readonly int Jump = Animator.StringToHash("Jump");

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _runnerService = ServiceLocator.Get<RunnerService>();

            _animator.gameObject.AddComponent<AnimatorSpy>()
                .controller = this;

            var spline = _runnerService.Spline;
            SplineUtility.GetNearestPoint(spline.Spline,spline.transform.InverseTransformPoint(transform.position), out var nearest, out _splinePosition);
        }

        private void Update()
        {
            if (CanMove) {
                if (_inputBridgeService.LeftMoveIsDown) {
                    MoveLeft();
                }

                if (_inputBridgeService.RightMoveIsDown) {
                    MoveRight();
                }
            }

            _splinePosition += _runnerService.CurrentSpeed * Time.deltaTime / _runnerService.Spline.CalculateLength();
            Vector3 rotation = _runnerService.Spline.EvaluateTangent(_splinePosition);
            _characterTransform.mainTransform.forward = rotation; 
            _characterTransform.mainTransform.position = _runnerService.Spline.EvaluatePosition(_splinePosition);
        }

        private void MoveLeft()
        {
            if (_currentRoadIndex < 0 || _isLineSwitchInProgress)
                return;

            _isLineSwitchInProgress = true;
            
            _animator.SetTrigger(Jump);
            _currentRoadIndex -= 1;
            _characterTransform.bodyTransform
                .DOLocalMoveX(_runnerService.MoveDistance * _currentRoadIndex, 0.25f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _isLineSwitchInProgress = false);
            
            DOTween.Kill(_animator.transform);
            _animator.transform.localRotation = Quaternion.Euler(0, -60, 0);
            _animator.transform.DOLocalRotate(Vector3.zero, 0.25f).SetDelay(0.25f);
        }

        private void MoveRight()
        {
            if (_currentRoadIndex > 0 || _isLineSwitchInProgress)
                return;
            
            _isLineSwitchInProgress = true;
            
            _animator.SetTrigger(Jump);
            _currentRoadIndex += 1;

            _characterTransform.bodyTransform
                .DOLocalMoveX(_runnerService.MoveDistance * _currentRoadIndex, 0.25f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _isLineSwitchInProgress = false);
            
            DOTween.Kill(_animator.transform);
            _animator.transform.localRotation = Quaternion.Euler(0, 60, 0);
            _animator.transform.DOLocalRotate(Vector3.zero, 0.25f).SetDelay(0.25f);
        }

        private void OnStepEvent()
        {
            _stepParticles.Play();
        }
        
        private class AnimatorSpy : MonoBehaviour
        {
            public RunnerController controller;
            
            public void OnStepEvent() => controller.OnStepEvent();
        }
    }
}