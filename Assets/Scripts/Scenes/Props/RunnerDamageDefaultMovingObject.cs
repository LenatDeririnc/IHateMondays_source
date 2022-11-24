using Plugins.ServiceLocator;
using Services;
using Tools.ReadOnlyAttribute;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes.Props
{
    public class RunnerDamageDefaultMovingObject : MovingObjectBase
    {
        [SerializeField] protected int _roadIndex = 0;
        [SerializeField] protected float _speed;
        [ReadOnly][SerializeField] private float _splinePosition;
        private SplineContainer _spline;
        private RunnerService _runnerService;

        protected override void Awake()
        {
            base.Awake();
            _runnerService = ServiceLocator.Get<RunnerService>();
            _spline = _runnerService.Spline;
            SplineUtility.GetNearestPoint(_spline.Spline,_spline.transform.InverseTransformPoint(transform.position), out var nearest, out _splinePosition);
            SnapToSpline();
        }

        protected override void MoveProcess()
        {
            _splinePosition += Time.deltaTime * -_speed / _spline.CalculateLength();
            SnapToSpline();
        }

        private void SnapToSpline()
        {
            _transform.position = _spline.EvaluatePosition(_splinePosition);
            _transform.forward = -_spline.EvaluateTangent(_splinePosition);
            _transform.position += _transform.right * (_roadIndex * _runnerService.MoveDistance);
        }
    }
}