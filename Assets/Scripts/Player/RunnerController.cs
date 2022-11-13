using System.Collections;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using UnityOverrides;

namespace Player
{
    public class RunnerController : PlayerBase
    {
        [SerializeField] private CharacterControllerDecorator _characterTransform;
        [SerializeField] public Transform CameraView;
        [SerializeField] public Transform CameraPosition;
        [SerializeField] private float moveDistance = 10f;

        private int _currentRoadIndex = 0;
        
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
            
            _characterTransform.Move(_characterTransform.transform.forward * (Time.deltaTime * _runnerService.CurrentSpeed));
        }

        private void MoveLeft()
        {
            if (_currentRoadIndex < 0)
                return;
            
            _currentRoadIndex -= 1;
            _characterTransform.Move(-_characterTransform.transform.right * moveDistance);
        }

        private void MoveRight()
        {
            if (_currentRoadIndex > 0)
                return;
            
            _currentRoadIndex += 1;
            _characterTransform.Move(_characterTransform.transform.right * moveDistance);
        }

        public void Rotate(Transform aroundWhat, int angle)
        {
            var resultPosition = aroundWhat.position + aroundWhat.right * _currentRoadIndex * moveDistance;
            _characterTransform.SetPosition(resultPosition);
            _characterTransform.Rotate(_characterTransform.transform.up, angle);
        }
    }
}