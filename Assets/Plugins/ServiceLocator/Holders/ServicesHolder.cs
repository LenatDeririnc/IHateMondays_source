using System.Collections.Generic;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Plugins.ServiceLocator.Holders
{
    public class ServicesHolder : MonoBehaviour
    {
        [SerializeField] protected List<Service> _servicesList = new();

        private bool _isInited;

        private List<IAwakeService> _awakes = new();
        private List<IUpdateService> _updates = new();
        private List<ITerminateService> _terminates = new();

        public virtual void Init()
        {
            transform.parent = null;
            _isInited = true;

            for (var index = 0; index < _servicesList.Count; index++)
            {
                var service = _servicesList[index];

                if (service == null) {
                    Debug.LogError("Empty field on service holder!", this);
                    continue;
                }
                
                var type = service.GetType();
                ServiceLocator.Set(type, service);

                if (service is IAwakeService awakeService) _awakes.Add(awakeService);
                if (service is IUpdateService updateService) _updates.Add(updateService);
                if (service is ITerminateService terminateService) _terminates.Add(terminateService);
            }
        }

        public void Awake()
        {
            Init();
            for (var index = 0; index < _awakes.Count; index++)
            {
                _awakes[index].AwakeService();
            }
        }

        public void Update()
        {
            for (var index = 0; index < _updates.Count; index++)
            {
                _updates[index].UpdateService();
            }
        }

        public void OnDestroy()
        {
            if (!_isInited)
                return;

            for (var index = 0; index < _terminates.Count; index++)
            {
                _terminates[index].TerminateService();
            }
        }
        
    }
}