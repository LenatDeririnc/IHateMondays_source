using System.Collections.Generic;
using DG.Tweening;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using Scenes;
using UnityEngine;

namespace Services
{
    public class RoomsService : Service, IAwakeService
    {
        [SerializeField] private Room _currentRoom;
        [SerializeField] public float RoomEnterDuration = 1f;
        [SerializeField] public Ease RoomEnterEase;
        [SerializeField] public Ease RoomExitEase;
        [SerializeField] public float RoomEnterEndValue = -10f;

        public Room[] rooms;

        private Dictionary<int, Room> _roomsByids = new Dictionary<int, Room>();


        public void AwakeService()
        {
            foreach (var r in rooms) {
                _roomsByids[r.ID] = r;
                r.Construct(this, r.ID == _currentRoom.ID);
            }
        }

        public void EnterRoom(int id)
        {
            if (_currentRoom.ID == id)
                return;
            
            if (_currentRoom != null) {
                _currentRoom.OnExitRoom();
            }

            _currentRoom = _roomsByids[id];
            _currentRoom.OnEnterRoom();
        }
    }
}