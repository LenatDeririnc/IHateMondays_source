using DG.Tweening;
using UnityEngine;

namespace Fungus
{
    public class ContinueIconAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _arrowTransform;
        [SerializeField] private float distance;
        [SerializeField] private float duration;

        private float startPos = 0f;

        private void Awake()
        {
            startPos = _arrowTransform.localPosition.x;
            var seq = DOTween.Sequence();
            seq.Append(_arrowTransform.DOLocalMoveX(startPos + distance, duration / 2).SetEase(Ease.InCubic));
            seq.Append(_arrowTransform.DOLocalMoveX(startPos, duration / 2).SetEase(Ease.OutCubic));
            seq.SetLoops(-1);
        }
    }
}