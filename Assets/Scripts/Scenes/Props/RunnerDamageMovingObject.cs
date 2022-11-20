using System;
using Plugins.ServiceLocator;
using Services;
using Tools.ReadOnlyAttribute;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes.Props
{
    public class RunnerDamageMovingObject : MonoBehaviour
    {
        [SerializeField] private int _roadIndex = 0;
        [SerializeField] private float _speed;
        [ReadOnly][SerializeField] private float _splinePosition;
        private SplineContainer _spline;
        private bool isMoving = false;
        private Transform _transform;
        private RunnerService _runnerService;

        private void Awake()
        {
            _runnerService = ServiceLocator.Get<RunnerService>();
            _transform = transform;
            _spline = _runnerService.Spline;
            SplineUtility.GetNearestPoint(_spline.Spline,_spline.transform.InverseTransformPoint(transform.position), out var nearest, out _splinePosition);
        }

        private void Update()
        {
            _transform.position = ((Vector3) _spline.EvaluatePosition(_splinePosition));
            _transform.forward = -_spline.EvaluateTangent(_splinePosition);
            _transform.position += _transform.right * (_roadIndex * _runnerService.MoveDistance);
            
            if (!isMoving)
                return;

            _splinePosition += Time.deltaTime * -_speed / _spline.CalculateLength();
        }

        public void StartMove()
        {
            isMoving = true;
        }
    }
}