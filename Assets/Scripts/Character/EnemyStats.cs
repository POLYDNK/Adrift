using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{

    [SerializeField]

    public int difficultyAdjustment; // can be positive or negative. Will adjust enemy ATK, HIT, CRIT, and AVO parameters depending on difficulty setting.
    public int difficultySetting; //5 difficulties from -2 (Very Easy) to +2 (Very Hard). diffcultyAdjustment = difficultySetting * 3


    // Start is called before the first frame update
    void Start()
    {

        // Set health and morale
        HP = getMaxHP();

        //Morale = MoraleMAX;
        
        //Temporary name attributed to enemy characters: Will need to have functionality added later
        Name = "Angery Doggo";

        difficultyAdjustment = difficultySetting * 3;
    }

    // Update is called once per frame
    void Update()
    {

        AVO = ((getSpeed()*3 + getLuck()) / 2) + (2 * (Morale / 5)) + difficultyAdjustment;
        
    }

    // HP changed (either taking damage (negative) or healing (positive))
    public void ememyAdjustHP(int change)
    {
        HP += change;

        if(HP < 0){
            HP = 0;
        }

        if(HP > getMaxHP()){
            HP = getMaxHP();
        }


    }


    //Attack the player, possibly with a critical hit
    //Note: Critical hits triple the total damage
    //Note: diffcultyAdjustment should be in the range of -6 to +6
    public int EnemyAttack(CharacterStats target){

        HIT = ((((getDexterity() * 3 + getLuck()) / 2) + (2 * (Morale / 5))) - target.AVO) + difficultyAdjustment;
        CRIT = ((((getDexterity() / 2) - 5) + (Morale / 5)) - target.getLuck()) + difficultyAdjustment;

        if(determineCRIT(CRIT)){

            ATK = (((getStrength() + (Morale / 5) + weaponBonus()) - target.getDefense()) + difficultyAdjustment) * 3; //CRITICAL HIT!
            //target.adjustHP(-ATK);

        }
        else if(determineHIT(HIT)){
            ATK = (((getStrength() + (Morale / 5) + weaponBonus()) - target.getDefense()) + difficultyAdjustment); //HIT!
            //target.adjustHP(-ATK);

        }
        else{

            ATK = 0; //Miss...
            
        }


        return ATK;
    }

}
