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
    private bool hasMoved;

    public EnemyAIController(BattleEngine be)
    {
        // Get battle engine script
        battleScript = be;
    }

    public IEnumerator PerformTurn(Character character, float moveTime, float endTime)
    {
        // Setup
        myCharacter = character;
        hasMoved    = false;
        currentGrid = myCharacter.myGrid.GetComponent<Grid>();

        Debug.Log("performTurn: moving to best tile");
        
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
            hasMoved = true;
        }

        //Debug.Log("moveToBestTile: moving to tile " + targetTile.GetComponent<TileScript>().position.ToString());

        // Move time
        yield return new WaitForSecondsRealtime(moveTime);

        battleScript.EndTurn();
        battleScript.LogicUpdate();

        // End time
        yield return new WaitForSecondsRealtime(endTime);
    }
}
