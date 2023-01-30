using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : EquippableItem
{


    [FormerlySerializedAs("Durability")] public int durability; //Each use of this weapon will decrease its durability by 1. When this reaches 0, the weapon breaks.
    public bool isBroken; //When broken, a weapon grant no bonuses until repaired


    ///  PASSIVE WEAPON ABILITIES (false by default) ///
    
    public bool speedSlash = false; //if the unit's SPD - target's SPD >= 5 , +2 ATK for every 5 SPD faster than target
    public bool strongSiphon = false; //when using Siphon, user will heal 20% more HP
    public bool deadlyPierce = false; //when using Pierce, CRIT +15
    public bool safeSwap = false; //When using LifeSwap, uses healing bonus (gain more HP or take less damage)
    public bool strongSoul = false; //When using SoulBash, if user is at full HP, bonus damage = MAX HP
    public bool lastStand = false; //When using Persistance, if user is at 1 HP, gauranteed critical hit
    public bool shiningStar = false; //When using Shooting Star, HIT/CRIT + 20
    public bool betterHeal = false; //When using LightHeal, baseHP is doubled
    public bool certainty = false; //When using QuickAttack, attack does not miss
    public bool earlyGambit = false; //When using Gambit, accuracy is better earlier
    public bool lateGambit = false; //when using Gambit, accuracy is better later

    [FormerlySerializedAs("MGT")] [SerializeField]
    public int mgt; //Might. Total attack is increased/decreased with this stat.
    [FormerlySerializedAs("DurabilityMAX")] public int durabilityMax;
    public List<Ability> abilities; //abilities attached to this weapon
    public GameObject model;

    

    // Start is called before the first frame update
    void Start()
    {

        durability = durabilityMax;
        isBroken = false;

        
    }


    public void WeaponDamage(){

        if(isBroken){
            return;
        }

        durability--;
        if(durability == 0){
            isBroken = true;
        }
    }

    public void WeaponDamage(int value){

        if(isBroken){
            return;
        }

        durability -= value;
        if(durability <= 0){
            durability = 0;
            isBroken = true;
        }
    }
}
