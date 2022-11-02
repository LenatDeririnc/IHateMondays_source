using System;
using System.Collections;
using System.Threading;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace SceneManager
{
    public class SceneLoader : MonoBehaviour
    {
        private LoadingCurtain _curtain;
        private CancellationTokenSource cts;

        public bool isLoadDone { get; private set; } = false;
        
        public void Construct(LoadingCurtain curtain)
        {
            _curtain = curtain;
        }

        public void LoadScene(SceneLink sceneLink, bool fastLoad = false, Action onLoad = null)
        {
            isLoadDone = false;
            if (fastLoad)
            {
                LoadSceneAsync(sceneLink, () => {
                    _curtain.Hide();
                    onLoad?.Invoke();
                });
                return;
            }
            _curtain.Show(() => LoadSceneAsync(sceneLink, () =>
            {
                _curtain.Hide();
                onLoad?.Invoke();
            }));
        }

        private void LoadSceneAsync(SceneLink sceneLink, Action action)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(LoadSceneCoroutine(sceneLink, action));
        }

        private Coroutine _coroutine;
        private IEnumerator LoadSceneCoroutine(SceneLink sceneLink, Action onLoaded = null)
        {
            AsyncOperation waitSceneAsync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneLink.sceneName);

            while (!waitSceneAsync.isDone)
                yield return null;

            onLoaded?.Invoke();
            isLoadDone = true;
        }
    }
}