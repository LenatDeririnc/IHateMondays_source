using UnityEngine;

public class PickUp : InteractElement
{
    public bool IsThrowFromInventory;
    [SerializeField] private Inventory _inventory;
    [SerializeField] public Sprite _sprite;

    public override void Use()
    {
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

    private void Start()
    {
        
    }
}
