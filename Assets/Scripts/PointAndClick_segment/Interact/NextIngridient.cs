using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class NextIngridient : InteractElement
{
    [SerializeField] Inventory _inventory;
    [SerializeField] private InteractElement _ingridient;
    [SerializeField] private bool _isFinal;
    [SerializeField] private InteractElement _result;
    [SerializeField] MouseTarget _target;

    public override void Use()
    {
        if (_target.InventoryPicked.PropName == _ingridient.PropName)
        {
            var obj = Instantiate(_ingridient, transform.position, Quaternion.identity);
            int i = 0;
            foreach (var item in _inventory.interactElements.ToArray())
            {
                if (obj.PropName == item.PropName)
                {
                    _inventory.Buttons[i].GetComponent<Image>().sprite = _inventory._spriteDefault;
                    if (!_isFinal)
                    {
                        _target.InventoryPicked = null;
                    }
                    _inventory.InventoryPanelUpdate(true, item);
                }
                i++;
            }
            obj.gameObject.AddComponent<Deletion>();
        }

        if (_isFinal)
        {
            if (_target.InventoryPicked.PropName == _ingridient.PropName)
            {
                Instantiate(_result, transform.position + new Vector3(0, 0.35f, 0), Quaternion.identity);
            }
            var delete = FindObjectsOfType<Deletion>();

            foreach (var obj in delete)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
