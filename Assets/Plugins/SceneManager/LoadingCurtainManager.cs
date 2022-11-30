using System;
using UnityEngine;

namespace SceneManager
{
    public class LoadingCurtainManager : MonoBehaviour
    {
        [SerializeField] private CurtainInfo[] _curtainList;

        public LoadingCurtainBase GetCurtain(CurtainType type)
        {
            foreach (var curtainInfo in _curtainList)
            {
                if (curtainInfo.type == type)
                    return curtainInfo.curtain;
            }

            return null;
        }

        public void ResetAllCurtains()
        {
            foreach (var curtainInfo in _curtainList) {
                curtainInfo.curtain.Reset();
            }
        }
        
        [Serializable]
        public struct CurtainInfo
        {
            public CurtainType type;
            public LoadingCurtainBase curtain;
        }
    }
}