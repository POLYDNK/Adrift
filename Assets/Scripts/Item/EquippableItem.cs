using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType //defining equipment types
{
  MeleeWeapon,
  Armor,
  Hat,
  Ring,
  Amulet,
  Bracelet,
  Shoes,
  Aura
}

// Weapons give different abilites and modify stats

// Armor gives higher DEF depending on a user's STR

//different types of accessories give different bonuses to the unit
//1: Hat, grants +STR/DEF/SPD/DEX
//2: Ring, grants +ATK/HIT/CRIT/AVO
//3: Amulet, grants +HP / +LCK
//4: Bracelet, grants +AP / +LCK
//5: Shoes, grants +MV
//6: Aura, can grant any of the above stats

//[CreateAssetMenu]//allows EquippableItem to be manually created in unity project editor
//inherited classes now have [CreateAssetMenu]
public class EquippableItem : Item //extends Item class - has scriptableObject
{
  //used for equipment parameter bonus that will be applied to character stats when equipped
  public List<StatModifier> modifiers;
  [Space]
  public EquipmentType EquipmentType;

  public void applyModifiers(CharacterStats character) {
    foreach(StatModifier modifier in modifiers) {
      character.addModifier(modifier);
    }
  }
}
