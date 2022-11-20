using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes
{
    public class BusAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _busTransform;
        [SerializeField] private SplineContainer _spline;
        [SerializeField] [Range(0, 1)] private float _splinePosition;
        [SerializeField] private float _duration = 1;
        [SerializeField] private Ease _ease;

        private void Update()
        {
            _busTransform.position = _spline.EvaluatePosition(_splinePosition);
            Vector3 rotation = _spline.Spline.EvaluateTangent(_splinePosition);
            _busTransform.forward = rotation;
        }

        public Sequence StartMove()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => _splinePosition, _ => _splinePosition = _, 1, _duration).SetEase(_ease));
            return sequence;
        }
    }
}
