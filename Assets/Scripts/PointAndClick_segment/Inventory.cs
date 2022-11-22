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
     public Sprite _spriteDefault;

    public void PickItemFromInventory(int index)
    {
        if (index < interactElements.Count)
        {
            _mouseTarget.InventoryPicked = interactElements[index];
        }
    }

    public void InventoryPanelUpdate(bool isTrow = false, InteractElement interact = null)
    {
        if (!isTrow)
        {
            InventUp();
        }
        else
        {
            interactElements.Remove(interact);
            InventUp(true);
        }
        CheckIngridients();
    }

    private void InventUp(bool withNull = false)
    {
        int i = 0;

        foreach (PickUp element in interactElements)
        {
            Buttons[i].GetComponent<Image>().sprite = element._sprite;
            i++;
        }

        if (withNull)
        {
            for (int j = 0; j < interactElements.Count + 1; j++)
            {
                try
                {
                    Buttons[j].GetComponent<Image>().sprite = interactElements[j].GetComponent<PickUp>()._sprite;
                }
                catch
                {
                    Buttons[j].GetComponent<Image>().sprite = _spriteDefault;
                }
            }
        }
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
