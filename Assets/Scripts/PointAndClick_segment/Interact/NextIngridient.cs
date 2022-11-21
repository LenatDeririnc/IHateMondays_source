using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextIngridient : InteractElement
{
    [SerializeField] private InteractElement _ingridient;
    [SerializeField] private bool _isFinal;
    [SerializeField] private InteractElement _result;
    [SerializeField] MouseTarget _target;

    public override void Use()
    {
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
        else
        {
            if (_target.InventoryPicked.PropName == _ingridient.PropName)
            {
                var obj = Instantiate(_ingridient, transform.position, Quaternion.identity);
                Destroy(obj.GetComponent<PickUp>());
                obj.gameObject.AddComponent<Deletion>();
            }
        }
    }
}
