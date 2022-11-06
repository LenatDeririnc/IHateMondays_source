using UnityEngine;

namespace Scenes.Props
{
    public class Roads : MonoBehaviour
    {
        [SerializeField] private Transform[] _roads;
        [SerializeField] private int _defaultIndex;

        public Transform[] Values => _roads;
        public int Length => Values.Length;
        public int DefaultIndex => _defaultIndex;

        public Transform this[int index] => _roads[index];
    }
}