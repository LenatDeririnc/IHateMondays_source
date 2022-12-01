using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float _startIngerval = 1f;
        [SerializeField] private float _duration = 1;
        [SerializeField] private float _interval = 5f;

        private void Start()
        {
            var seq = DOTween.Sequence();
            seq.AppendInterval(_startIngerval);
            seq.Append(_group.DOFade(1, _duration));
            seq.AppendInterval(_interval);
            seq.Append(_group.DOFade(0, _duration));
        }
    }
}