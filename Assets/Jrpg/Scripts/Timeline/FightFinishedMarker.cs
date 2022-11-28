using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Jrpg.Scripts.Timeline
{
    public class FightFinishedMarker : Marker, INotification
    {
        public PropertyName id => "FightFinishedMarker";
    }
}