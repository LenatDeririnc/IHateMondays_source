using UnityEngine;

namespace Scenes
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private int _id;

        public int ID => _id;

        [SerializeField] private GameObject _roomMesh;

        public bool isActive { get; private set; } = false;

        public void OnEnterRoom()
        {
            _roomMesh.SetActive(true);
            isActive = true;
        }

        public void OnExitRoom()
        {
            _roomMesh.SetActive(false);
            isActive = false;
        }
    }
}