using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : InteractElement
{
    [SerializeField] private string _interactedItem;
    [SerializeField] private GameObject _switchToGO;
    [SerializeField] private MouseTarget _mouseTarget;
    [SerializeField] SoundsContainer _soundsContainer;
    [SerializeField] private int _soundIndex;
    public Transform _spawnTransform;
    public Vector3 RotationOffset;
    public Vector3 OffsetPosition;

    public override void Use()
    {
        if (_mouseTarget.InventoryPicked != null && _mouseTarget.InventoryPicked.PropName == _interactedItem)
        {
            _soundsContainer.PlaySound(_soundIndex);
            gameObject.SetActive(false);
            var obj = Instantiate(_switchToGO, _spawnTransform.position + OffsetPosition, Quaternion.identity);
            obj.transform.Rotate(RotationOffset);
        }
    }
}
