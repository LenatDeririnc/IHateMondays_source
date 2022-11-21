using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : Open
{
    [SerializeField] protected Inventory _inventory;
    [SerializeField] protected MouseTarget _mouseTarget;

    public override void Use()
    {
        base.Use();
    }

    protected override void OpenClose()
    {
        if (_mouseTarget.InventoryPicked?.PropName == "LockerKey")
        {
            base.OpenClose();
            _mouseTarget.InventoryPicked = null;
        }
        else
        {
            Debug.Log("No key");
        }
    }
}
