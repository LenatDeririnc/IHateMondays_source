using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Jrpg.Scripts.Timeline
{
    public class FadeOutMusicMarker : Marker, INotification
    {
        public float duration;
        
        public PropertyName id => "FadeOutMusicMarker";
    }
}