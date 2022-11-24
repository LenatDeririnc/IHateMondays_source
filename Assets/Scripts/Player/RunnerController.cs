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

        public GameObject RenderersGameObject => _renderersGameObject;

        public Renderer[] Renderers => _renderers;

        public Animator Animator => _animator;

        private int _currentRoadIndex = 0;
        private float _splinePosition = 0f;
        
        private InputBridgeService _inputBridgeService;
        private RunnerService _runnerService;
        
        private Coroutine _damageCoroutine;
        private static readonly int Jump = Animator.StringToHash("Jump");

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _runnerService = ServiceLocator.Get<RunnerService>();

            var spline = _runnerService.Spline;
            SplineUtility.GetNearestPoint(spline.Spline,spline.transform.InverseTransformPoint(transform.position), out var nearest, out _splinePosition);
        }

        private void Update()
        {
            if (_inputBridgeService.LeftMoveIsDown) {
                MoveLeft();
            }

            if (_inputBridgeService.RightMoveIsDown) {
                MoveRight();
            }

            _splinePosition += _runnerService.CurrentSpeed * Time.deltaTime / _runnerService.Spline.CalculateLength();
            Vector3 rotation = _runnerService.Spline.EvaluateTangent(_splinePosition);
            _characterTransform.mainTransform.forward = rotation; 
            _characterTransform.mainTransform.position = _runnerService.Spline.EvaluatePosition(_splinePosition);
        }

        private void MoveLeft()
        {
            if (_currentRoadIndex < 0)
                return;
            
            _animator.SetTrigger(Jump);
            _currentRoadIndex -= 1;
            _characterTransform.bodyTransform.DOLocalMoveX(_runnerService.MoveDistance * _currentRoadIndex, 0.1f);
        }

        private void MoveRight()
        {
            if (_currentRoadIndex > 0)
                return;
            
            _animator.SetTrigger(Jump);
            _currentRoadIndex += 1;
            _characterTransform.bodyTransform.DOLocalMoveX(_runnerService.MoveDistance * _currentRoadIndex, 0.1f);
        }
    }
}