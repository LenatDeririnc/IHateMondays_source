using System.Collections;
using UnityEngine;

public class PickUp : InteractElement
{
    public bool IsLocked { get; set; }
    public bool IsThrowFromInventory;
    public bool IsLockedForTime;
    [SerializeField] private Inventory _inventory;
    [SerializeField] public Sprite _sprite;
    [SerializeField] private GameObject _ifLocked_ChangeToObject;
    public Vector3 _thisItemOffsetPosition;
    public Vector3 _thisItemRotation;

    public override void Use()
    {
        if(IsLocked) { return; }
        AddItemToInventory();
    }

    private void AddItemToInventory()
    {
        _inventory.interactElements.Add(this);
        _inventory.InventoryPanelUpdate();
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        _inventory = FindObjectOfType<Inventory>();
    }

    private void Update()
    {

    }

    public IEnumerator Timer(float lockTime)
    {
        IsLocked = true;
        yield return new WaitForSeconds(lockTime);
        Instantiate(_ifLocked_ChangeToObject,gameObject.transform.position,gameObject.transform.rotation);
        gameObject.SetActive(false);
        IsLocked = false;
        Debug.Log($"{gameObject.name} unlocked");
    }
}
