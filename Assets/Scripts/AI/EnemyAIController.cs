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
    [SerializeField] public CharacterStats myCharacter;
    [SerializeField] public FollowPath movePath;

    // Private Vars
    private GridBehavior currentGrid;
    private PathTreeNode tilesInRange;

    void performTurn()
    {

    }

    IEnumerator moveToTile(TileScript tile)
    {
        yield break;
    }

    TileScript getBestTile()
    {
        TileScript bestTile = null;

        return bestTile;
    }

    int calculateHeuristicValue(TileScript tile, GridBehavior grid)
    {
        int score = 0;
        Ability currentAbility;

        // Get Current Grid
        //currentGrid = myCharacter.myGrid.GetComponent<GridBehavior>();

        // Get Ability
        currentAbility = myCharacter.basicAttack;

        // Search the tiles within the range of this ability
        tilesInRange = grid.GetAllPathsFromTile(tile, currentAbility.range);

        

        return score;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
