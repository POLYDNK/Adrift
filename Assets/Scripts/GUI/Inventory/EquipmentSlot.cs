
using UnityEngine.Serialization;

public class EquipmentSlot : ItemSlot //extends ItemSlot class
{
  [FormerlySerializedAs("EquipmentType")] public EquipmentType equipmentType;

  protected override void OnValidate()
  {
    base.OnValidate(); //call Onvalidate function from base (itemslot) class
    gameObject.name = equipmentType.ToString() + " Slot"; //names value of equipment slots with equipmenttype
  }
}
