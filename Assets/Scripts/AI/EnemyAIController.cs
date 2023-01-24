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
    [SerializeField] public BattleEngine battleScript = null;
    [SerializeField] public FollowPath movePath;

    // Private Vars
    private GridBehavior currentGrid;
    private PathTreeNode tilesInRange;
    private List<GameObject> tilesToSearch;
    private List<GameObject> validTargets;
    private bool hasMoved;

    public IEnumerator performTurn(float endTime)
    {
        hasMoved = false;
        currentGrid = myCharacter.myGrid.GetComponent<GridBehavior>();

        Debug.Log("performTurn: moving to best tile");
        StartCoroutine(moveToBestTile(2.0f));

        if (battleScript == null)
        {
            battleScript = GameObject.Find("BattleEngine").GetComponent<BattleEngine>();;
        }

        battleScript.endTurn();

        // Return after waiting
        yield return new WaitForSecondsRealtime(endTime);
    }

    IEnumerator moveToBestTile(float moveTime)
    {
        // Reset valid targets
        validTargets = new List<GameObject>();

        // Search surrounding tiles for the best tile to move to
        GameObject targetTile = getBestTile(6);

        // Perform move
        Vector2Int newPosition = targetTile.GetComponent<TileScript>().position;

        if (myCharacter.gridPosition != newPosition)
        {
            currentGrid.MoveTowardsTile(myCharacter.gridPosition, newPosition, 
                                        false, myCharacter.MV.baseValue);
            hasMoved = true;
        }

        Debug.Log("moveToBestTile: moving to tile " + targetTile.GetComponent<TileScript>().position.ToString());

        // Return after waiting
        yield return new WaitForSecondsRealtime(moveTime);
    }

    GameObject getBestTile(int searchRange)
    {
        int highScore = int.MinValue;
        GameObject bestTile = null;

        GameObject myTile = currentGrid.GetTileAtPos(myCharacter.gridPosition);
        tilesInRange = currentGrid.GetAllPathsFromTile(myTile, searchRange, true);
        tilesToSearch = tilesInRange.GetAllTiles();

        foreach (GameObject tile in tilesToSearch)
        {
            PathTreeNode tilePath = tile.GetComponent<TileScript>().pathRef;

            // Get tile range from tile
            if (tilePath != null)
            {
                int tRange = tilePath.tileRange;

                // See whether current tile is within movement range
                bool inRange = (tRange > myCharacter.MV.baseValue);

                // Calculate the heuristic value of the current tile
                int newScore = calculateHeuristicValue(tile, currentGrid, inRange);

                // Check whether this new tile is the current best
                if (newScore > highScore)
                {
                    highScore = newScore;
                    bestTile = tile;
                }
            }
        }

        Debug.Log("getBestTile: found that the tile " + bestTile.GetComponent<TileScript>().position.ToString() + " is best");

        return bestTile;
    }

    int calculateHeuristicValue(GameObject tile, GridBehavior grid, bool inMoveRange)
    {
        int score = 0;
        Ability currentAbility;

        // Bonus score if within move range
        if (inMoveRange)
        {
            score += 25;
        }

        // Get Current Grid
        //currentGrid = myCharacter.myGrid.GetComponent<GridBehavior>();

        // Get Ability
        currentAbility = myCharacter.basicAttack;

        // Search the tiles within the range of this ability
        PathTreeNode abilityPathTree = grid.GetAllPathsFromTile(tile, currentAbility.range);
        List<GameObject> tiles = abilityPathTree.GetAllTiles();

        foreach (GameObject _tile in tiles)
        {
            // Get data from tile
            var tileScript = _tile.GetComponent<TileScript>();
            
            // If there's an enemy on the tile, then raise the score
            if (tileScript.characterOn != null)
            {
                if (isAlly(tileScript.characterOn) == false)
                {
                    score = currentAbility.baseDMG + currentAbility.baseHP;
                }
            }
        }
        
        Debug.Log("calculateHeuristicValue: the score on tile " + tile.GetComponent<TileScript>().position.ToString() +
            " is " + score.ToString());

        return score;
    }

    // Helper function to determine allies and enemys
    bool isAlly(GameObject characterObj)
    {
        // Get the script of the other character
        var otherCharScript = characterObj.GetComponent<CharacterStats>();

        // Compare the alliance of self and other and then return it
        return myCharacter.isPlayer() == otherCharScript.isPlayer();
    }
}
