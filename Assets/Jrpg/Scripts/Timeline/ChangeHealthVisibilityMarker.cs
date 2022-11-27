using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Jrpg.Scripts.Timeline
{
    public class ChangeHealthVisibilityMarker : Marker, INotification
    {
        public bool show;
        
        public PropertyName id => "ChangeHealthVisibilityMarker";
    }
}