using System;
using System.Collections;
using DG.Tweening;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace SceneManager
{
    public class SceneLoader : MonoBehaviour
    {
        private LoadingCurtainManager _curtainManager;

        public Action OnStartLoad;

        private LoadingCurtainBase _currentCurtain;
        private AsyncOperation _currentLoadingScene;
        private bool _isSceneActivated;

        public void Construct(LoadingCurtainManager curtainManager)
        {
            _curtainManager = curtainManager;
        }

        public void LoadScene(SceneLink sceneLink, CurtainType curtainType = CurtainType.AlphaTransition)
        {
            _isSceneActivated = false;
            OnStartLoad?.Invoke();

            _currentCurtain = _curtainManager.GetCurtain(curtainType);
            _currentCurtain.Show();
            
            _currentLoadingScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneLink.sceneName);
            _currentLoadingScene.allowSceneActivation = false;
        }

        private void Update()
        {
            if (!_isSceneActivated && _currentCurtain && _currentCurtain.CanActivateScene)
            {
                _currentLoadingScene.allowSceneActivation = true;
                _isSceneActivated = true;
                
                // Короткая задержка, так как если сделать hide в тот же момент что и активация сцены,
                // анимация будет очень дёрганная из за Lag Spike'а во время активации
                DOTween.Sequence()
                    .InsertCallback(0.1f, () => _currentCurtain.Hide())
                    .SetUpdate(true);
            }
        }
    }
}