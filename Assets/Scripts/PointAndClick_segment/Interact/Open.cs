using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open : InteractElement
{
    public override void Use()
    {
        OpenClose();
    }

    protected virtual void OpenClose()
    {
        IsUsed = !IsUsed;

        if (IsUsed)
        {
            Debug.Log("OpenAnimation");
        }
        else
        {
            Debug.Log("CloseAnimation");
        }
    }
}
