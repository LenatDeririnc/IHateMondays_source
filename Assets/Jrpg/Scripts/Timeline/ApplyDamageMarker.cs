using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[Serializable]
public class ApplyDamageMarker : Marker, INotification
{
    [FormerlySerializedAs("text")] public string customText;
    public int damageAmount;
    public JrpgTarget target;
    public AudioClip sfx;

    public PropertyName id => "ApplyDamageMarker";
}
