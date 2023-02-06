/// @author: Bryson Squibb
/// @date: 01/22/2023
/// @description: this script controls the behavior of an NPC.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController
{
    // Component References
    [SerializeField] public Character myCharacter;
    [SerializeField] public BattleEngine battleScript = null;

    // Private Vars
    private Grid currentGrid;
    private GameObject currentTile;
    private List<ValidTarget> validTargets = null;
    private float timer;

    // --------------------------------------------------------------
    // @desc: Default constructor
    // @arg: be - Reference to the battle engine script
    // --------------------------------------------------------------
    public EnemyAIController(BattleEngine be)
    {
        // Get battle engine script
        battleScript = be;
    }

    // --------------------------------------------------------------
    // @desc: Default constructor
    // @arg: character - The currently active character to take a turn
    // @arg: moveTime  - Wait time after moving
    // @arg: endTime   - Wait time after acting
    // @ret: IEnumerator - Used to enable waiting/yielding functionality
    // --------------------------------------------------------------
    public IEnumerator PerformTurn(Character character, float moveTime, float endTime)
    {
        // Setup
        myCharacter = character;
        currentGrid = myCharacter.myGrid.GetComponent<Grid>();

        Debug.Log("performTurn: moving to best tile");
        
        Move();

        // Move time
        yield return new WaitForSecondsRealtime(moveTime);

        PerformAction();

        //battleScript.EndTurn();
        //battleScript.LogicUpdate();

        // End time
        yield return new WaitForSecondsRealtime(endTime);
    }

    // --------------------------------------------------------------
    // @desc: Moves a character towards the tile with the highest generated
    // heat value provided by the HeatmapGenerator class
    // --------------------------------------------------------------
    private void Move()
    {
        // Generate Heatmap
        HeatmapGenerator.GenerateHeatmap(battleScript.aliveUnits, battleScript.activeUnit);

        // Get the best tile from the heatmap generator
        TileScript targetTile = HeatmapGenerator.GetBestTile();

        // Perform move
        Vector2Int newPosition = targetTile.position;

        // Check whether the char actually needs to move before doing do
        if (myCharacter.gridPosition != newPosition)
        {
            currentGrid.MoveTowardsTile(myCharacter.gridPosition, newPosition, 
                                        false, myCharacter.mv.baseValue);

            battleScript.Moved();
            //battleScript.EndMoveUnit(myCharacter.gridPosition);
            
            //battleScript.MoveUnit(newPosition);
        }

        currentTile = currentGrid.GetTileAtPos(myCharacter.gridPosition);

        //Debug.Log("moveToBestTile: moving to tile " + targetTile.GetComponent<TileScript>().position.ToString());
    }

    // --------------------------------------------------------------
    // @desc: Attempts to perform an action
    // --------------------------------------------------------------
    private void PerformAction()
    {
        ValidTarget abilityToUse = SelectAbilityAndTarget();

        if (abilityToUse != null)
        {
            // Set ability to use
            battleScript.SelectAbility(abilityToUse.GetAbility());

            // Get pos of target
            Vector2Int targetPos = abilityToUse.GetTarget().gridPosition;

            // Call battle engine to perform action
            battleScript.ActUnit(targetPos);

            // Set acted in the battle engine
            battleScript.Acted();
        }
        else
        {
            // If the AI cannot find a valid action, then end turn
            battleScript.EndTurn();
        }
    }

    // --------------------------------------------------------------
    // @desc: Goes through each valid target for each abilty - and then
    // selects the character/ability pair with the highest score
    // @ret: ValidTarget - A class that contains a valid target and 
    // an ability to target it with
    // --------------------------------------------------------------
    private ValidTarget SelectAbilityAndTarget()
    {
        ValidTarget targetToReturn = null;
        int highScore = int.MinValue;

        // Time logging
        timer = Time.time;
        Debug.Log("SelectAbilityAndTarget: Looking for best target...");

        // Get valid targets
        GetAllValidTargets();

        // Iterate through all valid targets to find the best one
        foreach (ValidTarget target in validTargets)
        {
            // If the power attribute is highest, then
            // we'll select this target, instead
            if (target.power > highScore)
            {
                // Set new valid target
                targetToReturn = target;

                // Set new highscore
                highScore = target.power;
            }
        }

        /* **** Random targeting ****
        System.Random random = new System.Random();
        int index = random.Next(validTargets.Count);
        targetToReturn = validTargets[index];
        */

        if (targetToReturn != null)
        {
            Debug.Log("Selected ability "
                + targetToReturn.GetAbility().displayName
                + " to target "
                + targetToReturn.GetTarget().displayName
                + "(power:" + targetToReturn.power + ")");
        }
        else
        {
            Debug.Log("Cannot find any valid targets! (retuning null)");
        }

        Debug.Log("SelectAbilityAndTarget: Completed target search (time elapsed: " + (Time.time-timer).ToString() + "s)");

        return targetToReturn;
    }

    // --------------------------------------------------------------
    // @desc: Populates the validTargets array with every target/ability
    // pair that the active character has
    // --------------------------------------------------------------
    private void GetAllValidTargets()
    {
        // Create list of valid targets
        validTargets = new List<ValidTarget>();

        // Populate the list for all abilities
        foreach(Ability ability in myCharacter.GetBattleAbilities())
        {
            GetTargetsForSingleAbility(ability);
        }
    }

    // --------------------------------------------------------------
    // @desc: Gets every valid target that a single ability can target
    // and then adds them to the list of valid targets
    // --------------------------------------------------------------
    private void GetTargetsForSingleAbility(Ability ability)
    {
        // Get data from ability
        int searchRange = ability.range;

        Debug.Log("Gettings targets for ability " + ability.displayName);

        // Get units within range of the ability
        PathTreeNode tilesInRange = currentGrid.GetAllPathsFromTile(currentTile, searchRange, true);
        List<Character> targets = tilesInRange.GetAllUnits();
        
        Debug.Log("Number of units in range: " + targets.Count.ToString());

        // Do this for each character
        foreach (Character target in targets)
        {
            if (target != null )
            {
                // Determine whether char is a valid target
                if (IsValidTarget(ability, target))
                {
                    ValidTarget newTarget = new ValidTarget(ability, myCharacter, target);
                    Debug.Log(newTarget.ToString());

                    // If so, then add a new valid target to the list
                    validTargets.Add(newTarget);
                }
            }
        }
    }

    private bool IsValidTarget(Ability a, Character target)
    {
        bool isValidTarget = false;

        // Alliance check
        if (a.friendly == IsAlly(myCharacter, target))
        {
            // AP Check
            if (myCharacter.ap >= a.costAP)
            {
                isValidTarget = true;
            }
        }

        return isValidTarget;
    }

    // --------------------------------------------------------------
    // @desc: Helper function to determine allies and enemys
    // @arg: charA - character to compare against
    // @arg: charB
    // @arg: Whether charA and charB are on the same team/crew
    // --------------------------------------------------------------
    public static bool IsAlly(Character charA, Character charB)
    {
        // Compare the alliance of self and other and then return it
        return charA.IsPlayer() == charB.IsPlayer();
    }

    protected internal class ValidTarget
    {
        private Ability ability = null;
        private Character caster = null;
        private Character target = null;
        public int power = int.MinValue;

        public ValidTarget(Ability _ability, Character caster, Character target)
        {
            ability = _ability;
            this.caster = caster;
            this.target = target;
            power = HeatmapGenerator.abilityImpact(_ability, caster, target);
        }

        public string ToString()
        {
            return caster.displayName + " can be targeted by the ability " + ability.displayName;
        }

        public Character GetTarget()
        {
            return target;
        }

        public Ability GetAbility()
        {
            return ability;
        }
    }
}
