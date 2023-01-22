using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Aura : EquippableItem
{
    [SerializeField]
    public int HealModifier;
    public int ATKmodifier;
    public int HITmodifier;
    public int CRITmodifier;
    public int AVOmodifier;

    //return either ATK (0), HIT (1), or CRIT (2)
    public int battleBonus(int type){

        switch(type){
            case 0:
                return ATKmodifier;
            case 1:
                return HITmodifier;
            case 2:
                return CRITmodifier;
            default:
                return 0; //error
        }

    }

}
