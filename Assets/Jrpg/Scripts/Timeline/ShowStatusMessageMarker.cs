using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[Serializable]
public class ShowStatusMessageMarker : Marker, INotification
{
    public string text;
    [FormerlySerializedAs("audio")] public AudioClip sfx;

    public PropertyName id => "ShowStatusMessageMarker";
}
