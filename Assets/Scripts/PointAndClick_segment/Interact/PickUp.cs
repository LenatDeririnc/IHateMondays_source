using System.Collections;
using UnityEngine;

public class PickUp : InteractElement
{
    public bool IsLocked { get; set; }
    public bool IsThrowFromInventory;
    public bool IsLockedForTime;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private SoundsContainer _soundsContainer;
    [SerializeField] private int _soundIndex;
    [SerializeField] public Sprite _sprite;
    [SerializeField] private GameObject _ifLocked_ChangeToObject;
    public Vector3 _thisItemOffsetPosition;
    public Vector3 _thisItemRotation;

    public override void Use()
    {
        if (IsLocked) { return; }
        _soundsContainer.PlaySound(_soundIndex);
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

    public IEnumerator Timer(float lockTime, GameObject ligth = null, string propName = "")
    {
        IsLocked = true;

        if (ligth != null)
        {
            ligth.SetActive(true);
        }

        if (propName !="")
        {
            if (propName == "Toster")
            {
                _soundsContainer.PlaySound(9);
            }
        }

        yield return new WaitForSeconds(lockTime);

        if (propName !="")
        {
            if (propName == "Toster")
            {
                _soundsContainer.PlaySound(8);
            }
        }

        Instantiate(_ifLocked_ChangeToObject, gameObject.transform.position, gameObject.transform.rotation);
        gameObject.SetActive(false);
        IsLocked = false;

        if (ligth != null)
        {
            ligth.SetActive(false);
        }
        Debug.Log($"{gameObject.name} unlocked");
    }
}
