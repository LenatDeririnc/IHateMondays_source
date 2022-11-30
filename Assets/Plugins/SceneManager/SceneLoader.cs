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

        private SceneLink _nextScene;
        private LoadingCurtainBase _currentCurtain;
        private AsyncOperation _currentLoadingScene;
        private bool _isSceneLoadingStarted;
        private bool _isSceneActivated;

        public void Construct(LoadingCurtainManager curtainManager)
        {
            _curtainManager = curtainManager;
        }

        public void LoadScene(SceneLink sceneLink, CurtainType curtainType = CurtainType.AlphaTransition)
        {
            _isSceneActivated = false;
            OnStartLoad?.Invoke();

            _nextScene = sceneLink;
            _currentCurtain = _curtainManager.GetCurtain(curtainType);
            
            _currentCurtain.Show();
            
            _isSceneLoadingStarted = false;
        }

        private void Update()
        {
            // Ожидание получения разрешения на загрузку от занавески, так как если запустить одновременно, то будет заметен лаг
            if (_currentCurtain && _currentCurtain.CanLoadScene && !_isSceneLoadingStarted)
            {
                _isSceneLoadingStarted = true;
                _currentLoadingScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_nextScene.sceneName);
                _currentLoadingScene.allowSceneActivation = false;
            }
            
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