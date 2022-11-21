using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractProp : InteractElement
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private string[] _interactArray;
    [SerializeField] private GameObject _spawnAnchor;
    [SerializeField] protected MouseTarget _mouseTarget;
    [SerializeField] private Sprite _defaultPanelKey;

    public override void Use()
    {
        foreach (var interact in _interactArray)
        {
            if (_mouseTarget.InventoryPicked?.PropName == interact)
            {
                var obj = Instantiate(_mouseTarget.InventoryPicked, _spawnAnchor.transform.position, Quaternion.identity);
                obj.gameObject.SetActive(true);

                if (obj.GetComponent<PickUp>().IsThrowFromInventory == true)
                {
                    int i = 0;
                    foreach (var item in _inventory.interactElements.ToArray())
                    {
                        if (obj.PropName == item.PropName)
                        {
                            _inventory.interactElements.Remove(item);
                            _inventory.Buttons[i].GetComponent<Image>().sprite = _defaultPanelKey;
                            _inventory.InventoryPanelUpdate();
                            _mouseTarget.InventoryPicked = null;
                        }

                        i++;
                    }
                }
            }
        }
    }
}
