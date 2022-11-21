using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : InteractElement
{
    [SerializeField] private string _interactedItem;
    [SerializeField] private GameObject _switchToGO;
    [SerializeField] private MouseTarget _mouseTarget;
    public Transform _spawnTransform;
    public Vector3 RotationOffset;
    public Vector3 OffsetPosition;

    public override void Use()
    {
        if (_mouseTarget.InventoryPicked != null && _mouseTarget.InventoryPicked.PropName == _interactedItem)
        {
            gameObject.SetActive(false);
            var obj = Instantiate(_switchToGO, _spawnTransform.position + OffsetPosition, Quaternion.identity);
            obj.transform.Rotate(RotationOffset);
        }
    }
}
