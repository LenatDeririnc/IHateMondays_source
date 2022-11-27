using UnityEngine;

[System.Serializable]
public class State
{
    [field: SerializeField] public InteractElement InteractElement { get; set; }
    public bool IsGot;

    public State(bool current, InteractElement element)
    {
        InteractElement = element;
        IsGot = current;
    }
}