using System;
using DG.Tweening;
using UnityEngine;

namespace SceneManager
{
    public class AlphaCurtain : LoadingCurtainBase
    {
        public CanvasGroup canvasGroup;
        public GameObject canvasGroupGameObject;
        public bool HideCurtainOnStart = true;
        public bool Enabled = true;

        public override bool CanActivateScene => _canActivateScene;

        private Coroutine _coroutine;
        private bool _canActivateScene;

        private void Start()
        {
            Enable(Enabled);

            if (HideCurtainOnStart)
            {
                Hide();
            }
        }

        public override void Reset()
        {
            canvasGroup.alpha = 0;
            Enable(false);
        }

        public override void Show(float speed, Action entireEndLoadAction = null)
        {
            Enable(true);
            canvasGroup.alpha = 0;
            _canActivateScene = false;
            DOTween.To(() => canvasGroup.alpha, _ => canvasGroup.alpha = _, 1, speed).onComplete += () =>
            {
                _canActivateScene = true;
                entireEndLoadAction?.Invoke();
            };
        }

        public override void Hide(float speed, Action entireEndLoadAction = null)
        {
            canvasGroup.alpha = 1;
            _canActivateScene = false;
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