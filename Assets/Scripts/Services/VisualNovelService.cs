using Fungus;
using Plugins.ServiceLocator;
using UnityEngine;

namespace Services
{
    public class VisualNovelService : Service
    {
        [SerializeField] private Stage _stage;
        public Stage Stage => _stage;
    }
}