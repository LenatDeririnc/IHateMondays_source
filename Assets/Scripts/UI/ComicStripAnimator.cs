using System;
using DG.Tweening;
using Fungus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ComicStripAnimator : MonoBehaviour
{
    public Strip[] strips;
    public UnityEvent onComplete;
    public CanvasGroup finishGroup;

    public bool isAllPagesShown;
    
    public Image overlayLines;
    public Image background;

    public float fadeInDuration;
    public float stripShowDuration;
    public float stripExplodeDuration = 0.5f;
    public float stripExplodeStaggerDuration = 0.1f;

    private int _currentStripIndex = 0;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        DOTween.Kill(this);
        gameObject.SetActive(true);

        finishGroup.blocksRaycasts = true;
        finishGroup.interactable = true;
        isAllPagesShown = false;
        _currentStripIndex = 0;
        
        foreach (var strip in strips)
        {
            strip.image.color = new Color(1f, 1f, 1f, 0f);
            strip.textGroup.alpha = 0f;
        }
        
        ScheduleNext();
    }

    public void Hide()
    {
        finishGroup.blocksRaycasts = false;
        finishGroup.interactable = false;
        Explode();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SkipToNext();
        }
    }

    public void SkipToNext()
    {
        ScheduleNext();
    }

    private void ScheduleNext()
    {
        if (_currentStripIndex >= strips.Length)
        {
            isAllPagesShown = true;
            onComplete.Invoke();
            return;
        }
        
        ShowStrip(strips[_currentStripIndex]);

        DOTween.Kill(this);
        DOTween.Sequence().InsertCallback(stripShowDuration, ScheduleNext)
            .SetTarget(this);

        _currentStripIndex++;
    }

    private void ShowStrip(Strip strip)
    {
        strip.image.DOFade(1f, fadeInDuration);
        strip.image.rectTransform.anchoredPosition = strip.imageStartPosition;
        strip.image.rectTransform.DOAnchorPos(strip.imageEndPosition, stripShowDuration);

        strip.textGroup.DOFade(1f, fadeInDuration);
    }

    private void Explode()
    {
        var delay = 0f;
        foreach (var strip in strips)
        {
            strip.imageContainer.DOPivot(strip.explodeDirection, stripExplodeDuration)
                .SetDelay(delay)
                .SetEase(Ease.InQuart);
            
            strip.textGroup.DOFade(0f, fadeInDuration)
                .SetEase(Ease.InQuart);

            delay += stripExplodeStaggerDuration;
        }

        overlayLines.DOFade(0f, stripExplodeDuration)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        finishGroup.blocksRaycasts = false;
        overlayLines.color = Color.clear;
        background.color = Color.clear;
    }

    [Serializable]
    public struct Strip
    {
        public Image image;
        public CanvasGroup textGroup;
        [FormerlySerializedAs("startPosition")] public Vector2 imageStartPosition;
        [FormerlySerializedAs("endPosition")] public Vector2 imageEndPosition;
        
        public RectTransform imageContainer;
        public Vector2 explodeDirection;
    }
}
