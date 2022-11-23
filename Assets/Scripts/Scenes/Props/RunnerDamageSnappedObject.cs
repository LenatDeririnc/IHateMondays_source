using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes.Props
{
    public class RunnerDamageSnappedObject : MonoBehaviour
    {
        [SerializeField] private int _roadIndex = 0;
        private SplineContainer _spline;
        private RunnerService _runnerService;

        private void Awake()
        {
            _runnerService = ServiceLocator.Get<RunnerService>();
            _spline = _runnerService.Spline;
            SplineUtility.GetNearestPoint(
                _spline.Spline,
                _spline.transform.InverseTransformPoint(transform.position), 
                out var nearest, out var splinePosition);
            
            transform.position = ((Vector3) _spline.EvaluatePosition(splinePosition));
            transform.forward = -_spline.EvaluateTangent(splinePosition);
            transform.position += transform.right * (_roadIndex * _runnerService.MoveDistance);
        }
    }
}