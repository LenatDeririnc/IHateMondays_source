using System.Collections.Generic;
using UnityEngine;

namespace SceneManager
{
    public class LoadingCurtainManager : MonoBehaviour
    {
        [SerializeField] private LoadingCurtainBase[] _curtainList;
        
        private readonly Dictionary<CurtainType, LoadingCurtainBase> _curtains 
            = new Dictionary<CurtainType, LoadingCurtainBase>();

        public void Construct()
        {
            foreach (var c in _curtainList) {
                _curtains[c.Type] = c;
            }
        }

        public LoadingCurtainBase GetCurtain(CurtainType type)
        {
            return _curtains[type];
        }
    }
}