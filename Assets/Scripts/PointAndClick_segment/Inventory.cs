using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public bool IsInventoryFull { get; private set; }
    [SerializeField] private InteractElement[] _sandwichArray;
    [SerializeField] private InteractProp _desk;

    public List<InteractElement> interactElements;
    public List<GameObject> Buttons;

    [SerializeField] private MouseTarget _mouseTarget;

    public void PickItemFromInventory(int index)
    {
        if (index < interactElements.Count)
            _mouseTarget.InventoryPicked = interactElements[index];
    }

    public void InventoryPanelUpdate()
    {
        int i = 0;

        foreach (PickUp element in interactElements)
        {
            Buttons[i].GetComponent<Image>().sprite = element._sprite;
            i++;
        }
        CheckIngridients();
    }

    private void CheckIngridients()
    {
        int i = 0;
        foreach (InteractElement element in interactElements)
        {
            foreach (var sandwich in _sandwichArray)
            {
                if (element.PropName == sandwich.PropName)
                {
                    i++;
                }
            }
        }

        if (i >= 3)
        {
            foreach (InteractElement element in interactElements)
            {
                element.GetComponent<PickUp>().IsThrowFromInventory = true;
            }
            _desk.ChangeDeskArray();
        }
    }
}
