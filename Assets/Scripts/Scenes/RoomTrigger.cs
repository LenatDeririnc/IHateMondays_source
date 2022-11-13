using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes
{
    public class RoomTrigger : MonoBehaviour
    {
        [SerializeField] private Room _room;
        private RoomsService _roomService;

        private void Awake()
        {
            _roomService = ServiceLocator.Get<RoomsService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _roomService.EnterRoom(_room.ID);
        }
    }
}