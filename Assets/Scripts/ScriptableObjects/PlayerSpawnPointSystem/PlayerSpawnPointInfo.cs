using System;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace Scenes.PlayerSpawner
{
    [CreateAssetMenu(fileName = "SpawnObject", menuName = "ScriptableObjects/Scene/SpawnObject", order = 0)]
    public class PlayerSpawnPointInfo : ScriptableObjectWithID
    {
        [SerializeField] private SceneLink _scene;
        public SceneLink Scene => _scene;
    }
}