using System.Collections.Generic;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using Scenes;

namespace Services
{
    public class RoomsService : Service, IAwakeService
    {
        public Room[] rooms;

        private Dictionary<int, Room> _roomsByids = new Dictionary<int, Room>();

        private Room _currentRoom;

        public void AwakeService()
        {
            foreach (var r in rooms) {
                _roomsByids[r.ID] = r;
                if (!r.isActive)
                    r.OnExitRoom();
            }
        }

        public void EnterRoom(int id)
        {
            if (_currentRoom != null) {
                _currentRoom.OnExitRoom();
            }

            _currentRoom = _roomsByids[id];
            _currentRoom.OnEnterRoom();
        }
    }
}