using System;
using System.Collections;
using UnityEngine;

namespace SceneManager
{
    public class AlphaCurtain : LoadingCurtainBase
    {
        public override CurtainType Type => CurtainType.AlphaTransition;
        
        public CanvasGroup canvasGroup;
        public GameObject canvasGroupGameObject;
        public float fadeSpeed = 1;
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

        public override void Hide(Action loadSceneAction = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FadeIn(fadeSpeed, loadSceneAction));
        }

        public override void Show(Action loadSceneAction = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FadeOut(fadeSpeed, loadSceneAction));
        }

        public override void Hide(float speed, Action loadSceneAction = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FadeIn(speed, loadSceneAction));
        }

        public override void Show(float speed, Action loadSceneAction = null)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FadeOut(speed, loadSceneAction));
        }

        public void SetTransparency(float value)
        {
            canvasGroup.alpha = value;
        }

        public void Enable(bool value)
        {
            canvasGroupGameObject.SetActive(value);
            Enabled = value;
        }

        private IEnumerator FadeOut(float speed, Action loadSceneAction)
        {
            Enable(true);
            
            SetTransparency(0);
            
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += speed * Time.deltaTime;
                yield return null;
            }

            SetTransparency(1);

            loadSceneAction?.Invoke();
        }

        private IEnumerator FadeIn(float speed, Action loadSceneAction)
        {
            SetTransparency(1);
            
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= speed * Time.deltaTime;
                yield return null;
            }
            
            SetTransparency(0);

            loadSceneAction?.Invoke();
            Enable(false);
        }
    }
}