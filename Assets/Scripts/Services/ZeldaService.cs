using Player;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;

namespace Services
{
    public class ZeldaService : Service, IAwakeService
    {
        private ZeldaController _controller;

        public ZeldaController Controller => _controller;
        
        public void AwakeService()
        {
            _controller = (ZeldaController)ServiceLocator.Get<PlayerService>().Player;
        }
    }
}