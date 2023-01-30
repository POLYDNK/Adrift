using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    [SerializeField]

    public int difficultyAdjustment; // can be positive or negative. Will adjust enemy ATK, HIT, CRIT, and AVO parameters depending on difficulty setting.
    public int difficultySetting; //5 difficulties from -2 (Very Easy) to +2 (Very Hard). diffcultyAdjustment = difficultySetting * 3


    // Start is called before the first frame update
    void Start()
    {

        // Set health and morale
        hp = GetMaxHp();

        //Morale = MoraleMAX;
        
        //Temporary name attributed to enemy characters: Will need to have functionality added later
        displayName = "Angery Doggo";

        difficultyAdjustment = difficultySetting * 3;
    }

    // Update is called once per frame
    void Update()
    {

        avo = ((GetSpeed()*3 + GetLuck()) / 2) + (2 * (morale / 5)) + difficultyAdjustment;
        
    }

    // HP changed (either taking damage (negative) or healing (positive))
    public void EmemyAdjustHp(int change)
    {
        hp += change;

        if(hp < 0){
            hp = 0;
        }

        if(hp > GetMaxHp()){
            hp = GetMaxHp();
        }


    }


    //Attack the player, possibly with a critical hit
    //Note: Critical hits triple the total damage
    //Note: diffcultyAdjustment should be in the range of -6 to +6
    public int EnemyAttack(Character target){

        hit = ((((GetDexterity() * 3 + GetLuck()) / 2) + (2 * (morale / 5))) - target.avo) + difficultyAdjustment;
        crit = ((((GetDexterity() / 2) - 5) + (morale / 5)) - target.GetLuck()) + difficultyAdjustment;

        if(DetermineCrit(crit)){

            atk = (((GetStrength() + (morale / 5) + WeaponBonus()) - target.GetDefense()) + difficultyAdjustment) * 3; //CRITICAL HIT!
            //target.adjustHP(-ATK);

        }
        else if(DetermineHit(hit)){
            atk = (((GetStrength() + (morale / 5) + WeaponBonus()) - target.GetDefense()) + difficultyAdjustment); //HIT!
            //target.adjustHP(-ATK);

        }
        else{

            atk = 0; //Miss...
            
        }


        return atk;
    }

}
