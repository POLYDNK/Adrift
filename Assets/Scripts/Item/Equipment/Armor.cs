using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Armor : EquippableItem
{
    public int RES; //Resistance. Total defense is increased/decreased with this stat.
    public int CON; //Constitution. Depending on this value compared to unit's STR, total movement allowed will be decreased, from 1 to 5.


    public new void applyModifiers(CharacterStats character)
    {
        //base.applyModifiers(character);
        //TODO: Armor should implement its custom modifiers here (below)

        //character.getDefense() += RES;
        character.addModifier(new StatModifier(StatType.MV, OpType.ADD, -1, RES, 1));

        if(character.getStrength() < CON){

            if(((CON - character.getStrength()) / 10) > 5){

                //-5 MV modifier
                character.addModifier(new StatModifier(StatType.MV, OpType.ADD, -1, -5, 1));

            }
            else{

                //((armor.CON - character.getStrength()) / 10) MV modifier
                float value = ((CON - character.getStrength()) / 10);
                character.addModifier(new StatModifier(StatType.MV, OpType.ADD, -1, -value, 1));
            }

        }
            
    }
    
}
