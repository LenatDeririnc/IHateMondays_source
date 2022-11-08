using UnityEngine;

public class PickUp : InteractElement
{
    [SerializeField] private Inventory _inventory;

    public override void Use()
    {
        AddItemToInventory();
    }

    private void AddItemToInventory()
    {
        _inventory.interactElements.Add(this);
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        _inventory = FindObjectOfType<Inventory>();
    }
}
