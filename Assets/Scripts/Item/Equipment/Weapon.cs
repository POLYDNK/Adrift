using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : EquippableItem
{


    public int Durability; //Each use of this weapon will decrease its durability by 1. When this reaches 0, the weapon breaks.
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

    [SerializeField]
    public int MGT; //Might. Total attack is increased/decreased with this stat.
    public int DurabilityMAX;
    public List<Ability> abilities; //abilities attached to this weapon
    public GameObject model;

    

    // Start is called before the first frame update
    void Start()
    {

        Durability = DurabilityMAX;
        isBroken = false;

        
    }


    public void weaponDamage(){

        if(isBroken){
            return;
        }

        Durability--;
        if(Durability == 0){
            isBroken = true;
        }
    }

    public void weaponDamage(int value){

        if(isBroken){
            return;
        }

        Durability -= value;
        if(Durability <= 0){
            Durability = 0;
            isBroken = true;
        }
    }
}
