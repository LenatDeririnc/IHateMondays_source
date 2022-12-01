using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class ArrowAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _direction;
        [SerializeField] private float distance;
        [SerializeField] private float duration;

        private Vector3 _startPos;
        private Sequence _seq;

        private void Awake()
        {
            _startPos = _transform.position;
            _seq = DOTween.Sequence();
            _seq.Append(_transform.DOMove(_startPos + _direction.forward * distance, duration / 2).SetEase(Ease.InCubic));
            _seq.Append(_transform.DOMove(_startPos, duration / 2).SetEase(Ease.OutCubic));
            _seq.SetLoops(-1);
        }

        private void OnDestroy()
        {
            _seq.Kill();
            _seq = null;
        }
    }
}