using DG.Tweening;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using UnityOverrides;

namespace Player
{
    public class RunnerController : PlayerBase
    {
        [SerializeField] private RunnerControllerDecorator _characterTransform;

        private int _currentRoadIndex = 0;
        private float _splinePosition = 0f;
        
        private InputBridgeService _inputBridgeService;
        private RunnerService _runnerService;
        
        private Coroutine _damageCoroutine;

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _runnerService = ServiceLocator.Get<RunnerService>();
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
            
            _currentRoadIndex -= 1;
            _characterTransform.bodyTransform.DOLocalMoveX(_runnerService.MoveDistance * _currentRoadIndex, 0.1f);
        }

        private void MoveRight()
        {
            if (_currentRoadIndex > 0)
                return;
            
            _currentRoadIndex += 1;
            _characterTransform.bodyTransform.DOLocalMoveX(_runnerService.MoveDistance * _currentRoadIndex, 0.1f);
        }
    }
}