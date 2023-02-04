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

    public EnemyAIController(BattleEngine be)
    {
        // Get battle engine script
        battleScript = be;
    }

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

    private void PerformAction()
    {
        ValidTarget abilityToUse = SelectAbilityAndTarget();

        if (abilityToUse != null)
        {
            // Set ability to use
            battleScript.SelectAbility(abilityToUse.a);

            // Get pos of target
            Vector2Int targetPos = abilityToUse.c.gridPosition;

            // Call battle engine to perform action
            battleScript.ActUnit(targetPos);
        }

        battleScript.Acted();
    }

    private ValidTarget SelectAbilityAndTarget()
    {
        ValidTarget targetToReturn = null;
        int highScore = int.MinValue;

        // Get valid targets
        GetAllValidTargets();

        // Iterate through all valid targets to find the best one
        foreach (ValidTarget target in validTargets)
        {
            // If the power attribute is highest, then
            // we'll select this target, instead
            if (target.power > int.MinValue)
            {
                targetToReturn = target;
            }
        }

        if (targetToReturn != null)
        {
            Debug.Log("Selected ability "
                + targetToReturn.a.displayName
                + " to target "
                + targetToReturn.c.displayName);
        }
        else
        {
            Debug.Log("Cannot find any valid targets! (retuning null)");
        }

        return targetToReturn;
    }

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

    private void GetTargetsForSingleAbility(Ability ability)
    {
        // Get data from ability
        int searchRange = ability.range;

        Debug.Log("Gettings targets for ability " + ability.displayName);

        // Get units within range of the ability
        PathTreeNode tilesInRange = currentGrid.GetAllPathsFromTile(currentTile, searchRange, true);
        List<Character> chars = tilesInRange.GetAllUnits();
        
        Debug.Log("Number of units in range: " + chars.Count.ToString());

        // Do this for each character
        foreach (Character _char in chars)
        {
            if (_char != null )
            {
                // Determine whether char is a valid target
                if (ability.friendly == IsAlly(myCharacter, _char))
                {
                    ValidTarget newTarget = new ValidTarget(ability, _char);
                    Debug.Log(newTarget.ToString());

                    // If so, then add a new valid target to the list
                    validTargets.Add(newTarget);
                }
            }
        }
    }

    // Helper function to determine allies and enemys
    public static bool IsAlly(Character charA, Character charB)
    {
        // Compare the alliance of self and other and then return it
        return charA.IsPlayer() == charB.IsPlayer();
    }

    protected internal class ValidTarget
    {
        public Ability a = null;
        public Character c = null;
        public int power = int.MinValue;

        public ValidTarget(Ability _ability, Character _char)
        {
            a = _ability;
            c = _char;
            power = _ability.baseDmg + _ability.baseHp;
        }

        public string ToString()
        {
            return c.displayName + " can be targeted by the ability " + a.displayName;
        }
    }
}
