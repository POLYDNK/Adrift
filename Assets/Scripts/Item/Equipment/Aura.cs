using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Aura : EquippableItem
{
    [FormerlySerializedAs("HealModifier")] [SerializeField]
    public int healModifier;
    [FormerlySerializedAs("ATKmodifier")] public int atKmodifier;
    [FormerlySerializedAs("HITmodifier")] public int hiTmodifier;
    [FormerlySerializedAs("CRITmodifier")] public int criTmodifier;
    [FormerlySerializedAs("AVOmodifier")] public int avOmodifier;

    //return either ATK (0), HIT (1), or CRIT (2)
    public int BattleBonus(int type){

        switch(type){
            case 0:
                return atKmodifier;
            case 1:
                return hiTmodifier;
            case 2:
                return criTmodifier;
            default:
                return 0; //error
        }

    }

}
