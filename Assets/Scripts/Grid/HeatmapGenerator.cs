using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeatmapGenerator
{
    // Private vars
    private static Grid currentGrid;
    private static Character activeChar;
    private static TileScript activeTile;

    public static void GenerateHeatmap(List<GameObject> allUnits, GameObject activeUnit)
    {
        // Get active char script
        activeChar = activeUnit.GetComponent<Character>();
        activeTile = activeChar.myGrid.GetComponent<Grid>().GetTileAtPos(activeChar.gridPosition).GetComponent<TileScript>();

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
                SingleAbilityHeat(unit, charScript.basicAttack, charScript.gridPosition);
            }
        }
    }

    static void AllAbilityHeat(GameObject unit)
    {

    }

    static void SingleAbilityHeat(GameObject unit, Ability ability, Vector2Int pos)
    {
        // Get data from ability
        int searchRange = ability.range;
        int power = ability.baseDmg + ability.baseHp;

        // Include enemy checking here plzzz
        if (ability.friendly == IsAlly(unit))
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
                    tileScript.heatVal = power;
                }
            }
        }
    }

    // Helper function to determine allies and enemys
    static bool IsAlly(GameObject characterObj)
    {
        // Get the script of the other character
        var otherCharScript = characterObj.GetComponent<Character>();

        // Compare the alliance of self and other and then return it
        return activeChar.IsPlayer() == otherCharScript.IsPlayer();
    }
}
