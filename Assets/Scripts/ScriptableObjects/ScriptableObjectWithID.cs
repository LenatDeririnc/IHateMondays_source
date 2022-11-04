using System;
using UnityEngine;

namespace Scenes
{
    [CreateAssetMenu(fileName = "ObjectID", menuName = "ScriptableObjects/Scene/ObjectID", order = 0)]
    public class ScriptableObjectWithID : ScriptableObject
    {
        [SerializeField] private string _id;
        public string ID => _id;
        private void Awake()
        {
            _id ??= Guid.NewGuid().ToString();
        }
        
        public override string ToString()
        {
            return ID;
        }

        public void RegenerateID()
        {
            _id = Guid.NewGuid().ToString();
        }
    }
}