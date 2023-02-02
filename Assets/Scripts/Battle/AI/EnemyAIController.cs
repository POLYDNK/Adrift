/// @author: Bryson Squibb
/// @date: 01/22/2023
/// @description: this script controls the behavior of an NPC.
/// It is meant to be attached to a character object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    // Component References
    [SerializeField] public Character myCharacter;
    [SerializeField] public BattleEngine battleScript = null;

    // Private Vars
    private Grid currentGrid;
    private bool hasMoved;

    public IEnumerator PerformTurn(float endTime)
    {
        hasMoved = false;
        currentGrid = myCharacter.myGrid.GetComponent<Grid>();

        Debug.Log("performTurn: moving to best tile");
        StartCoroutine(MoveToBestTile(2.0f));

        if (battleScript == null)
        {
            battleScript = GameObject.Find("BattleEngine").GetComponent<BattleEngine>();
        }

        battleScript.EndTurn();
        battleScript.LogicUpdate();

        // Return after waiting
        yield return new WaitForSecondsRealtime(endTime);
    }

    IEnumerator MoveToBestTile(float moveTime)
    {
        // Generate Heatmap
        HeatmapGenerator.GenerateHeatmap(battleScript.aliveUnits, battleScript.activeUnit);

        // Get the best tile from the heatmap generator
        TileScript targetTile = HeatmapGenerator.GetBestTile();

        // Perform move
        Vector2Int newPosition = targetTile.position;

        if (myCharacter.gridPosition != newPosition)
        {
            currentGrid.MoveTowardsTile(myCharacter.gridPosition, newPosition, 
                                        false, myCharacter.mv.baseValue);
            hasMoved = true;
        }

        Debug.Log("moveToBestTile: moving to tile " + targetTile.GetComponent<TileScript>().position.ToString());

        // Return after waiting
        yield return new WaitForSecondsRealtime(moveTime);
    }
}
