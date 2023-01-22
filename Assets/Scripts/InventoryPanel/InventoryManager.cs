
using UnityEngine;
//class to control what happens to item being equipped and unequipped
public class InventoryManager : MonoBehaviour
{
  [SerializeField] Inventory inventory;
  [SerializeField] EquipmentPanel equipmentPanel;

  private void Awake()
  {
    inventory.OnItemRightClickedEvent += EquipFromInventory; //right clicking in inventory triggers event
    equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipPanel;//right clicking in equip panel triggers event
  }
  private void EquipFromInventory(Item item) //used for right clicking, if event triggers, equip item from inventory
  {
    if (item is EquippableItem)
    {
      Equip((EquippableItem)item);
    }
  }
  private void UnequipFromEquipPanel(Item item) //once right clicked, event triggers and unequips from equipment panel
  {
    if (item is EquippableItem)
    {
      Unequip((EquippableItem)item);
    }
  }

  public void Equip(EquippableItem item)
  {
    if (inventory.RemoveItem(item)) //removes item from inventory
    {
      EquippableItem prevItem;
      if (equipmentPanel.AddItem(item, out prevItem))//add items to equpment panel
      {
        if (prevItem != null) //if equpment panel slot is not empty then add that item in inventory
        {
          inventory.AddItem(prevItem); //adds item back into inventory
        }
          inventory.RemoveItem(item);
      }
      else
      {
      inventory.AddItem(item); //if unable to equip item, return item back to inventory
      }
    }
  }

  public void Unequip(EquippableItem item)
  {
    if (!inventory.IsFull() && equipmentPanel.RemoveItem(item))//checks inventory if its not full, is able to remove item
    {
      inventory.AddItem(item); //add item in inventory
    }
  }

}
