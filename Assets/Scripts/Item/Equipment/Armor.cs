using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Armor : EquippableItem
{
    [FormerlySerializedAs("RES")] public int res; //Resistance. Total defense is increased/decreased with this stat.
    [FormerlySerializedAs("CON")] public int con; //Constitution. Depending on this value compared to unit's STR, total movement allowed will be decreased, from 1 to 5.


    public new void ApplyModifiers(Character character)
    {
        //base.applyModifiers(character);
        //TODO: Armor should implement its custom modifiers here (below)

        //character.getDefense() += RES;
        character.AddModifier(new StatModifier(StatType.Mv, OpType.Add, -1, res, 1));

        if(character.GetStrength() < con){

            if(((con - character.GetStrength()) / 10) > 5){

                //-5 MV modifier
                character.AddModifier(new StatModifier(StatType.Mv, OpType.Add, -1, -5, 1));

            }
            else{

                //((armor.CON - character.getStrength()) / 10) MV modifier
                float value = ((con - character.GetStrength()) / 10);
                character.AddModifier(new StatModifier(StatType.Mv, OpType.Add, -1, -value, 1));
            }

        }
            
    }
    
}
