using UnityEngine;

namespace Scenes.Props
{
    public class Paths : MonoBehaviour
    {
        [SerializeField] private Transform[] _paths;
        [SerializeField] private int _defaultIndex;

        public Transform[] Values => _paths;
        public int Length => Values.Length;
        public int DefaultIndex => _defaultIndex;

        public Transform this[int index] => _paths[index];
    }
}