using System.Collections;
using Plugins.ServiceLocator;
using Scenes.Props;
using Services;
using UnityEngine;
using UnityOverrides;

namespace Player
{
    public class RunnerController : PlayerBase
    {
        [SerializeField] private CharacterControllerDecorator _characterControllerDecorator;
        [SerializeField] private float moveLeftRightSpeed = 10;

        private Paths _paths;
        private int _currentRoadIndex;
        private bool _canUseInput = true;
        private float distanceDifference = 0.01f;

        private Coroutine _moveCoroutine;
        private Coroutine _damageCoroutine;

        private InputBridgeService _inputBridgeService;
        private RunnerService _runnerService;

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _runnerService = ServiceLocator.Get<RunnerService>();
            _paths = _runnerService.Paths;
            _currentRoadIndex = _paths.DefaultIndex;
            _characterControllerDecorator.SetPosition(_paths[_currentRoadIndex].position);
        }

        private void Update()
        {
            if (!_canUseInput)
                return;
            
            if (_inputBridgeService.LeftMoveIsDown && _currentRoadIndex > 0) {
                _moveCoroutine = StartCoroutine(MoveLeft());
                return;
            }

            if (_inputBridgeService.RightMoveIsDown && _currentRoadIndex < _paths.Length - 1) {
                _moveCoroutine = StartCoroutine(MoveRight());
                return;
            }
        }

        private IEnumerator MoveLeft()
        {
            _canUseInput = false;
            _currentRoadIndex -= 1;
            var distance = (_paths[_currentRoadIndex].position - _characterControllerDecorator.transform.position).magnitude;
            
            while (distance > distanceDifference) {
                var difference = moveLeftRightSpeed * Time.deltaTime;
                _characterControllerDecorator.Move(Vector3.left * difference);
                distance -= difference;
                yield return null;
            }
            _characterControllerDecorator.SetPosition(_paths[_currentRoadIndex].position);
            _canUseInput = true;
        }

        private IEnumerator MoveRight()
        {
            _canUseInput = false;
            _currentRoadIndex += 1;
            
            var distance = (_paths[_currentRoadIndex].position - _characterControllerDecorator.transform.position).magnitude;
            
            while (distance > distanceDifference) {
                var difference = moveLeftRightSpeed * Time.deltaTime;
                _characterControllerDecorator.Move(Vector3.right * difference);
                distance -= difference;
                yield return null;
            }
            _characterControllerDecorator.SetPosition(_paths[_currentRoadIndex].position);
            _canUseInput = true;
        }

        public override void ReceiveDamage()
        {
            if (_damageCoroutine != null) {
                StopCoroutine(_damageCoroutine);
            }
            _damageCoroutine = StartCoroutine(Damage());
        }

        private IEnumerator Damage()
        {
            _runnerService.SetSlownDownSpeed();
            yield return new WaitForSeconds(_runnerService.DamageSeconds);
            _runnerService.SetDefaultSpeed();
        }
    }
}