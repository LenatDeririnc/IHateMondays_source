using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : Open
{
    [SerializeField] protected Inventory _inventory;

    public override void Use()
    {
        base.Use();
    }

    protected override void OpenClose()
    {
        if (_inventory.FindInventoryItem("LockerKey"))
        {
            base.OpenClose();
        }
        else
        {
            Debug.Log("No key");
        }
    }
}
