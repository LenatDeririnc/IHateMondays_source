using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InteractProp : InteractElement
{
    [SerializeField] bool _isLockerProp;
    [SerializeField] float _lockTime;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private string[] _interactArray;
    [SerializeField] private GameObject _spawnAnchor;
    [SerializeField] protected MouseTarget _mouseTarget;
    [SerializeField] private Sprite _defaultPanelKey;
    [SerializeField] bool IsOverrideOffsetsByProp;
    [SerializeField] private GameObject _light;
    [Header("Only for microwave")]
    [SerializeField] private MicroStartButton _microStartButton;
    public Vector3 _thisItemOffsetPosition;
    public Vector3 _thisItemRotation;
    private bool _changed;
    private InteractElement _cash;

    public override void Use()
    {
        foreach (var interact in _interactArray)
        {
            if (_mouseTarget.InventoryPicked?.PropName == interact)
            {
                if (!IsOverrideOffsetsByProp)
                {
                    _thisItemOffsetPosition = _mouseTarget.InventoryPicked.GetComponent<PickUp>()._thisItemOffsetPosition;
                    _thisItemRotation = _mouseTarget.InventoryPicked.GetComponent<PickUp>()._thisItemRotation;
                }
                var obj = Instantiate(_mouseTarget.InventoryPicked, _spawnAnchor.transform.position + _thisItemOffsetPosition, Quaternion.identity);
                obj.transform.Rotate(_thisItemRotation);
                obj.gameObject.SetActive(true);

                if (_changed)
                {
                    obj.gameObject.AddComponent<Deletion>();
                }

                if (_isLockerProp && obj.GetComponent<PickUp>().IsLockedForTime)
                {
                    StartCoroutine(obj.GetComponent<PickUp>().Timer(_lockTime,_light,PropName));
                    Debug.Log($"{ obj.name} locked for {_lockTime}");
                }
                if (obj.IsPickUpOff)
                {
                    Destroy(obj.gameObject.GetComponent<PickUp>());
                }

                if (obj.GetComponent<PickUp>().IsThrowFromInventory == true)
                {
                    int i = 0;
                    foreach (var item in _inventory.interactElements.ToArray())
                    {
                        if (obj.PropName == item.PropName)
                        {
                            _cash = item;
                            _inventory.Buttons[i].GetComponent<Image>().sprite = _defaultPanelKey;
                            _mouseTarget.InventoryPicked = null;
                            _inventory.InventoryPanelUpdate(true, _cash);
                        }
                        i++;
                    }
                }

                CheckSandvitchInMicro(PropName, interact);
            }
        }
    }

    public void ChangeDeskArray()
    {
        _changed = true;
        _interactArray = new string[1];
        _interactArray[0] = "breadZhar";
    }

    private void CheckSandvitchInMicro(string propName, string sandvitch)
    {
       if(propName == "Micro" && sandvitch == "sammic")
        {
            _microStartButton.Use();
            gameObject.GetComponent<Animator>().SetBool("Switch", false);
        }
    }
}
