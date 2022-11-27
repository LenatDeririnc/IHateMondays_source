using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class StateGroup : MonoBehaviour
{
    [Multiline]
    public string StateText;
    public State[] states;
    public bool StateDone;
}
