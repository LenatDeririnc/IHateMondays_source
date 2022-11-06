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
        [SerializeField] private float moveSpeed;

        private Roads _roads;
        private int _currentRoadIndex;
        private bool _canUseInput = true;
        private float distanceDifference = 0.01f;

        private Coroutine _coroutine;

        private InputBridgeService _inputBridgeService;
        private RoadsService _roadsService;

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _roadsService = ServiceLocator.Get<RoadsService>();
            _roads = _roadsService.Roads;
            _currentRoadIndex = _roads.DefaultIndex;
            _characterControllerDecorator.SetPosition(_roads[_currentRoadIndex].position);
        }

        private void Update()
        {
            if (!_canUseInput)
                return;
            
            if (_inputBridgeService.LeftMoveIsDown && _currentRoadIndex > 0) {
                _coroutine = StartCoroutine(MoveLeft());
                return;
            }

            if (_inputBridgeService.RightMoveIsDown && _currentRoadIndex < _roads.Length - 1) {
                _coroutine = StartCoroutine(MoveRight());
                return;
            }
        }

        private IEnumerator MoveLeft()
        {
            _canUseInput = false;
            _currentRoadIndex -= 1;
            var distance = (_roads[_currentRoadIndex].position - _characterControllerDecorator.transform.position).magnitude;
            
            while (distance > distanceDifference) {
                var difference = moveSpeed * Time.deltaTime;
                _characterControllerDecorator.Move(Vector3.left * difference);
                distance -= difference;
                yield return null;
            }
            _characterControllerDecorator.SetPosition(_roads[_currentRoadIndex].position);
            _canUseInput = true;
        }

        private IEnumerator MoveRight()
        {
            _canUseInput = false;
            _currentRoadIndex += 1;
            
            var distance = (_roads[_currentRoadIndex].position - _characterControllerDecorator.transform.position).magnitude;
            
            while (distance > distanceDifference) {
                var difference = moveSpeed * Time.deltaTime;
                _characterControllerDecorator.Move(Vector3.right * difference);
                distance -= difference;
                yield return null;
            }
            _characterControllerDecorator.SetPosition(_roads[_currentRoadIndex].position);
            _canUseInput = true;
        }
    }
}