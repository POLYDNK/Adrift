using System;
using UnityEngine;
using UnityEngine.UI; //allows access to unity image class
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

//item slot is the square slots in the inventory panel.
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
  //[SerializeField] Item _item;
  [FormerlySerializedAs("Image")] [SerializeField] Image image; //availble in unity inspector, but also protected so that image is not easily changed,
  public event Action<Item> OnRightClickEvent;

  //private Image image
  private Item item; //items that will assign to each slow in inventory panel
  public Item Item
  {
    get { return item; }

    set {
          item = value;
          if (item == null) // if slot in inventory panel is empty, disables the sprite image component for that item
          {
            image.enabled = false;
          }
          else
          {
            image.sprite = item.icon; //enable items sprite icon to show in slot
            image.enabled = true;
          }
        }
    }
public void OnPointerClick(PointerEventData eventData)
{
  //used for right clicking on item or equipmentS
  //checks if right click is clicked.
  if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
  {
    //when right clicking in item slots, event triggers
      if (Item != null && OnRightClickEvent != null)
      OnRightClickEvent(Item);
      Debug.Log("Right button was Clicked");
  }
}


//protecting virtual onvalidate function to be used in equipmentSlot subclass
protected virtual void OnValidate() //unity function that is called specifically in  editor, triggers when specific value is changed, used to avoid manually changing values(images) for each slot
{
  if (image == null) //if image component is null, use get component to find respective image.
  {
    image = GetComponent<Image>();
  }
}

}
