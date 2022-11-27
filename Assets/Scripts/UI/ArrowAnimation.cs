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
        
        private void Awake()
        {
            _startPos = _transform.position;
            var seq = DOTween.Sequence();
            seq.Append(_transform.DOMove(_startPos + _direction.forward * distance, duration / 2).SetEase(Ease.InCubic));
            seq.Append(_transform.DOMove(_startPos, duration / 2).SetEase(Ease.OutCubic));
            seq.SetLoops(-1);
        }
    }
}