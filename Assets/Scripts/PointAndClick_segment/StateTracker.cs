using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum States { FindKey, PickKnife, FindIngridients, Slice, CookBread, CookSandwitch, CookInMicro }

public class StateTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentText;
    [SerializeField] private Inventory _inventory;
    [Space]
    [SerializeField] private List<PickUp> _interactElements;
   // [SerializeField] private State[] _statesArray;
    [SerializeField] private StateGroup[] _statesGroupArray;
    [SerializeField] private States _currentState;

    private void Start()
    {
        var objects = FindObjectsOfType<PickUp>();
        _interactElements = new List<PickUp>();
        _interactElements.AddRange(objects);
        //_statesArray = new State[_interactElements.Count];

        //for (int i = 0; i < _statesArray.Length; i++)
        //{
        //    _statesArray[i] = new State(false, _interactElements[i]);
        //}

        _currentText.text = _statesGroupArray[(int)_currentState].StateText;
    }

    public void StateUpdate(List<InteractElement> elements)
    {
        int currentStateIndex = 0;
        foreach (var state in _statesGroupArray)
        {
            int len = state.states.Length;
            int count = 0;
            foreach (var element in state.states)
            {
                foreach (var interact in _inventory.interactElements)
                {
                    if (element.InteractElement.PropName == interact.PropName)
                    {
                        element.IsGot = true;
                    }
                }
                if (!state.StateDone)
                {
                    if (element.IsGot)
                    {
                        count++;
                    }
                    if (state.states.Length == count)
                    {
                        state.StateDone = true;
                    }
                }
            }
        }

        for (int i = 0; i < _statesGroupArray.Length; i++)
        {
            if (_statesGroupArray[i].StateDone)
            {
                currentStateIndex++;
            }
            else
            {
                break;
            }
        }
        _currentState = (States)currentStateIndex;
        _currentText.text = _statesGroupArray[(int)_currentState].StateText;
    }
}