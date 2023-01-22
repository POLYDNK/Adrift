using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentPanel : MonoBehaviour
{
  [SerializeField] Transform equipmentSlotsParent;
  [SerializeField] EquipmentSlot[] equipmentSlots;

  public event Action<Item> OnItemRightClickedEvent;

private void Start()
{
  for (int i =0; i < equipmentSlots.Length; i++)
  {
    equipmentSlots[i].OnRightClickEvent += OnItemRightClickedEvent; //when item is right clicked, event triggers

  }
}
  private void OnValidate()
  {
    equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();

  }

  public bool AddItem(EquippableItem item, out EquippableItem prevItem)
  //bool to add/replace equippable item into specific equipment type slot
  {
    for (int i=0; i< equipmentSlots.Length; i++)
    {
      if (equipmentSlots[i].EquipmentType == item.EquipmentType) //checks slot/array for equipment type to be placed
      {
        prevItem = (EquippableItem)equipmentSlots[i].Item; //assign current item in slot to prevItem to be replaced/removed into inventorymanager class
        equipmentSlots[i].Item = item; //if slot is of same type of item, then allow
        return true;
      }
    }
      prevItem = null;
      return false; //if equipment slot is not of same type as equippable item, then don't allow
   }

     public bool RemoveItem(EquippableItem item) //bool to remove equippable item from slot,
    {
      for (int i=0; i< equipmentSlots.Length; i++)
      {
        if (equipmentSlots[i].Item == item) //checks equipment slot for item, since we are removing, type doesnt matter
        {
          equipmentSlots[i].Item = null; //return null into equipment slot, makes it empty
          return true;
        }
      }
        return false; //if already empty, leave empty
    }
}
