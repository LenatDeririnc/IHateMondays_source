using System;
using UnityEngine;

namespace SceneManager
{
    public abstract class LoadingCurtainBase : MonoBehaviour
    {
        public abstract CurtainType Type { get; }
        
        public abstract void Hide(Action loadSceneAction = null);
        public abstract void Show(Action loadSceneAction = null);
        public abstract void Hide(float speed, Action loadSceneAction = null);
        public abstract void Show(float speed, Action loadSceneAction = null);
    }
}