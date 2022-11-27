using System;
using DG.Tweening;
using UnityEngine;

namespace SceneManager
{
    public class AlphaCurtain : LoadingCurtainBase
    {
        public override CurtainType Type => CurtainType.AlphaTransition;

        public CanvasGroup canvasGroup;
        public GameObject canvasGroupGameObject;
        public bool HideCurtainOnStart = true;
        public bool Enabled = true;

        private Coroutine _coroutine;

        private void Start()
        {
            Enable(Enabled);

            if (HideCurtainOnStart)
            {
                Hide();
            }
        }

        public override void Show(float speed, Action entireEndLoadAction = null)
        {
            Enable(true);
            canvasGroup.alpha = 0;
            DOTween.To(() => canvasGroup.alpha, _ => canvasGroup.alpha = _, 1, speed).onComplete += () =>
            {
                entireEndLoadAction?.Invoke();
            };
        }

        public override void Hide(float speed, Action entireEndLoadAction = null)
        {
            canvasGroup.alpha = 1;
            DOTween.To(() => canvasGroup.alpha, _ => canvasGroup.alpha = _, 0, speed).onComplete += () =>
            {
                entireEndLoadAction?.Invoke();
                Enable(false);
            };
        }

        private void Enable(bool value)
        {
            canvasGroupGameObject.SetActive(value);
            Enabled = value;
        }
    }
}