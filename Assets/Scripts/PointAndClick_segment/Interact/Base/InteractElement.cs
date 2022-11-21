using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractElement : MonoBehaviour
{
    [field: SerializeField] public string PropName { get; set; }
    [field: SerializeField] public bool IsUsed { get; protected set; } = false;
    public bool IsPickUpOff;
    public abstract void Use();
}
