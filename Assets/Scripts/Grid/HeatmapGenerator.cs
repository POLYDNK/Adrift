using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeatmapGenerator
{
    // Private vars
    private static Grid currentGrid;
    private static Character activeChar;
    private static GameObject activeTileObj;
    private static TileScript activeTile;
    private static TileScript bestTile;
    private static int highScore;

    public static void GenerateHeatmap(List<GameObject> allUnits, GameObject activeUnit)
    {
        // Reset
        bestTile = null;
        highScore = int.MinValue;
        ResetHeatValues();

        // Get active char script
        activeChar = activeUnit.GetComponent<Character>();
        activeTileObj = activeChar.myGrid.GetComponent<Grid>().GetTileAtPos(activeChar.gridPosition);
        activeTile = activeTileObj.GetComponent<TileScript>();

        // Generate heat around the tiles of all units based on what
        // abilities the active character can use
        foreach (GameObject unit in allUnits)
        {
            // Do not include active char for ability search
            if (unit != activeUnit)
            {
                // Get character script
                Character charScript = unit.GetComponent<Character>();

                // Set current grid
                currentGrid = charScript.myGrid.GetComponent<Grid>();

                // Ability Heat
                AllAbilityHeat(charScript);
            }
        }

        // Generate extra heat for tiles the active char can move to
        currentGrid = activeChar.myGrid.GetComponent<Grid>();
        MovementHeat(20);
    }

    // Reset the heat value of all tiles to 0
    static void ResetHeatValues()
    {
        // Painfully reset the heat value by getting the tile's script (slow)
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in objects)
        {
            tile.GetComponent<TileScript>().heatVal = 0;
        }
    }

    // Heat generation for every action the active character can do
    static void AllAbilityHeat(Character charScript)
    {
        // Generate heat for every battle ability of the active character
        foreach(Ability ability in activeChar.GetBattleAbilities())
        {
            SingleAbilityHeat(charScript, ability, charScript.gridPosition);
        }
    }

    // Heat generation for a single ability
    static void SingleAbilityHeat(Character charScript, Ability ability, Vector2Int pos)
    {
        // Get data from ability
        int searchRange = ability.range;
        int power = ability.baseDmg + ability.baseHp;

        // Include enemy checking here plzzz
        if (ability.friendly == IsAlly(charScript))
        {
            // Get what tiles we need to touch
            GameObject myTile = currentGrid.GetTileAtPos(pos);
            PathTreeNode tilesInRange = currentGrid.GetAllPathsFromTile(myTile, searchRange, true);
            List<GameObject> tiles = tilesInRange.GetAllTiles();

            // Do this for every tile within range of the ability
            foreach (GameObject tile in tiles)
            {
                TileScript tileScript = tile.GetComponent<TileScript>();

                // If the tile doesn't have a character on it
                if (tileScript.hasCharacter == false || pos == activeTile.position)
                {
                    // Since the active character can use an ability from this square,
                    // we'll increase the heat of this tile
                    tileScript.heatVal += power;

                    // Check whether the high score is beaten
                    if (tileScript.heatVal > highScore)
                    {
                        highScore = tileScript.heatVal;
                        bestTile = tileScript;
                    }
                }
            }
        }
    }

    static void MovementHeat(int moveWeight)
    {
        // Get what tiles we need to touch
        GameObject myTile = currentGrid.GetTileAtPos(activeTile.position);
        PathTreeNode tilesInRange = currentGrid.GetAllPathsFromTile(activeTileObj, activeChar.mv.baseValue);
        List<GameObject> tiles = tilesInRange.GetAllTiles();

        // Do this for every tile within movement range
        foreach (GameObject tile in tiles)
        {
            // Get tile script
            TileScript tileScript = tile.GetComponent<TileScript>();

            // Since the active character can move to this tile,
            // we'll increase the heat of this tile
            tileScript.heatVal += moveWeight;

            // Check whether the high score is beaten
            if (tileScript.heatVal > highScore)
            {
                highScore = tileScript.heatVal;
                bestTile = tileScript;
            }
        }
    }

    // Helper function to determine allies and enemys
    private static bool IsAlly(Character charScript)
    {
        // Compare the alliance of self and other and then return it
        return activeChar.IsPlayer() == charScript.IsPlayer();
    }

    // Helper function to determine allies and enemys
    public static bool IsAlly(Character charA, Character charB)
    {
        // Compare the alliance of self and other and then return it
        return charA.IsPlayer() == charB.IsPlayer();
    }

    public static TileScript GetBestTile()
    {
        Debug.Log("HeatmapGenerator: the best tile has a score of " + highScore.ToString());

        return bestTile;
    }
}
