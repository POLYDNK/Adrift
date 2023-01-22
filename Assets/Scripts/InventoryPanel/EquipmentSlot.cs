
public class EquipmentSlot : ItemSlot //extends ItemSlot class
{
  public EquipmentType EquipmentType;

  protected override void OnValidate()
  {
    base.OnValidate(); //call Onvalidate function from base (itemslot) class
    gameObject.name = EquipmentType.ToString() + " Slot"; //names value of equipment slots with equipmenttype
  }
}
