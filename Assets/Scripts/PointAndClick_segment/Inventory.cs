using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InteractElement> interactElements;

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
}
