using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeleeTypeWeapon : Weapon
{

    [SerializeField]
    int type; //0 = Sword, 1 = Scimitar, 2 = Saber

    


    // Start is called before the first frame update
    void Start()
    {

        //Determine modifiers and default passives
        switch(type){


            case 0:
                //Swords generally have no modifiers, Sword users are all around units

                //Passives:
                strongSiphon = true;
                strongSoul = true;
                betterHeal = true;
                earlyGambit = true;


                break;

            case 1:
                //The scimitar weapon sacrifices defenses for more mobility and power;

                /* General modifiers:
                
                HP -5
                DEF -3
                SPD +5
                DEX +3
                LCK +2
                
                */

                //Passives:
                speedSlash = true;
                lastStand = true;
                shiningStar = true;
                certainty = true;
                

                break;
            case 2:

                //The saber weapon focuses on defensive/supportive tactics

                /* General modifiers:
                
                HP +5
                DEF +5
                SPD -3
                DEX -3
                LCK -2
                
                */

                //Passives:
                deadlyPierce = true;
                safeSwap = true;
                betterHeal = true;
                lateGambit = true;

                
                

                break;
            default:
                //weapons unspecified will have no changes
                
               

                break;
        }






        
    }





}
