using System;
using System.Collections;
using UnityEngine;

namespace SceneManager
{
    public class LoadingCurtain : MonoBehaviour
    {
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

        public void Show(Action loadSceneAction)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FadeOut(loadSceneAction));
        }

        public void Hide()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FadeIn());
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

        private IEnumerator FadeOut(Action loadSceneAction)
        {
            Enable(true);
            
            SetTransparency(0);
            
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += fadeSpeed * Time.deltaTime;
                yield return null;
            }

            SetTransparency(1);

            loadSceneAction();
        }

        private IEnumerator FadeIn()
        {
            SetTransparency(1);
            
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                yield return null;
            }
            
            SetTransparency(0);

            Enable(false);
        }
    }
}