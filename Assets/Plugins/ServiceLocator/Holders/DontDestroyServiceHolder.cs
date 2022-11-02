using Plugins.ServiceLocator.Tools.SimpleSingleton;
using UnityEngine;

namespace Plugins.ServiceLocator.Holders
{
    public class DontDestroyServiceHolder : ServicesHolder, ISingleton<DontDestroyServiceHolder>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()
        {
            if (ISingleton<DontDestroyServiceHolder>.Instance != null)
                return;
            
            var resource = Resources.Load("DontDestroyServices");
            var dontDestroyServices = Instantiate(resource) as GameObject;
            DontDestroyOnLoad(dontDestroyServices);
            Debug.Log("Dont Destroy Services Initialized");
        }
        
        public override void Init()
        {
            this.InitSingleton(this, base.Init);
        }
    }
}