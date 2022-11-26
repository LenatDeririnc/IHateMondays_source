using System;
using System.Collections;
using System.Threading;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace SceneManager
{
    public class SceneLoader : MonoBehaviour
    {
        private LoadingCurtainManager _curtainManager;
        private CancellationTokenSource cts;

        public bool isLoadDone { get; private set; } = false;
        
        public void Construct(LoadingCurtainManager curtainManager)
        {
            _curtainManager = curtainManager;
        }

        public void LoadScene(SceneLink sceneLink, bool fastLoad = false, Action onLoad = null, CurtainType curtainType = CurtainType.AlphaTransition)
        {
            isLoadDone = false;
            if (fastLoad)
            {
                LoadSceneAsync(sceneLink, () => {
                    _curtainManager.GetCurtain(curtainType).Hide();
                    onLoad?.Invoke();
                });
                return;
            }
            _curtainManager.GetCurtain(curtainType).Show(() => LoadSceneAsync(sceneLink, () =>
            {
                _curtainManager.GetCurtain(curtainType).Hide();
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