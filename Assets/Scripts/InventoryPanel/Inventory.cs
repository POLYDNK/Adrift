using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
  [SerializeField] List<Item> items;
  [SerializeField] Transform itemsParent;
  [SerializeField] ItemSlot[] itemSlots;

  public event Action<Item> OnItemRightClickedEvent;

private void Start()//Awake()
{
  for (int i =0; i < itemSlots.Length; i++)
  {
    itemSlots[i].OnRightClickEvent += OnItemRightClickedEvent; //when item is right clicked, event triggers
    RefreshUI();
  }
}

  private void OnValidate() //useful unity funtion to reduce manual input/change of values
  {
    if (itemsParent != null) //gets all itemslots attached, and automatically assigns it to correct itemslot
      itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
      RefreshUI(); //allows UI to update in unity without having to press play. Useful for testing
  }

  private void RefreshUI()
  {
    int i = 0;
    for (; i < items.Count && i < itemSlots.Length; i++) //assign an itemslot in inventory panel for every item
    {
      itemSlots[i].Item = items[i];
    }
    for (; i < itemSlots.Length; i++) //for each item slots on inventory panel that is empty, we set item slot to null to show no image
    {
      itemSlots[i].Item = null;
    }
  }

  public bool AddItem(Item item) //bool to add item to inventory
  {
    if (IsFull()) //checks if inventory is full, if full, return false
    {
      return false;

    }
    else
    {
    items.Add(item); //if not full, add items to item list
    RefreshUI();
    return true;
    }
  }

  public bool RemoveItem(Item item)//bool to remove item in inventory
    {
      if (items.Remove(item)) //if item was successfully removed, return true, else return false
      {
        RefreshUI();
        return true;
      }
      return false;
    }

    public bool IsFull() //bool to check if inventory is full, item slots is maxed at 18
    {
      return items.Count >= itemSlots.Length; //checks amount of items in list, if more than item slots, then inventory is full
    }
  }
