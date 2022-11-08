using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [field: SerializeField] public bool IsInventoryFull { get; private set; }

    public List<InteractElement> interactElements;
    public List<GameObject> Buttons;

    public bool FindInventoryItem(string itemName)
    {
        if (interactElements.Count > 0)
        {
            foreach (InteractElement element in interactElements)
            {
                if (element.PropName == itemName)
                { return true; }
            }
        }

        return false;
    }

    public void InventoryPanelUpdate()
    {
        int i = 0;

        foreach(PickUp element in interactElements)
        {
            Buttons[i].GetComponent<Image>().sprite = element._sprite;
            i++;
        }
    }
}
