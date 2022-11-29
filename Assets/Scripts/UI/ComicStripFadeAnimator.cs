using DG.Tweening;
using UnityEngine;

public class ComicStripFadeAnimator : MonoBehaviour
{
    public RectTransform targetTransform;

    public float duration;
    public Ease tweenEase;

    private void Awake()
    {
        targetTransform.gameObject.SetActive(false);
    }

    public void Show()
    {
        targetTransform.gameObject.SetActive(true);
        targetTransform.pivot = new Vector2(-0.5f, 0f);
        targetTransform.DOPivotX(0.5f, duration)
            .SetEase(tweenEase);
    }

    public void Hide()
    {
        targetTransform.pivot = new Vector2(0.5f, 0f);
        targetTransform.DOPivotX(1.5f, duration)
            .SetEase(tweenEase)
            .OnComplete(() => targetTransform.gameObject.SetActive(false));
    }
}
