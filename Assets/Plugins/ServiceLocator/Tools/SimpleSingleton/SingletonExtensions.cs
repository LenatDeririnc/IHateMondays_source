using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.ServiceLocator.Tools.SimpleSingleton
{
    public static class SingletonExtensions
    {
        public static void InitSingleton<T>(this ISingleton<T> singleton, T instance, Action firstInitAction = null) 
            where T : MonoBehaviour
        {
            if (ISingleton<T>.Instance == null)
            {
                ISingleton<T>.Instance = instance;
                instance.transform.parent = null;
                Object.DontDestroyOnLoad(instance.gameObject);
                firstInitAction?.Invoke();
            }
            else
            {
                Object.Destroy(instance);
                Object.Destroy(instance.gameObject);
            }
        }
    }
}