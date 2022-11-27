using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deletion : MonoBehaviour
{
    void OnEnable()
    {
        var g = gameObject.GetComponent<PickUp>();
        g.IsPickUpOff = true;
        g.enabled = false;
        Destroy(gameObject.GetComponent<PickUp>());
    }
}
