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
    private static float timer;

    public static void GenerateHeatmap(List<GameObject> allUnits, GameObject activeUnit)
    {
        timer = Time.time;
        Debug.Log("GenerateHeatmap: Generating heatmap...");

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

        Debug.Log("GenerateHeatmap: Heatmap generated (time elapsed: " + (Time.time-timer).ToString() + "s)");
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
                    tileScript.heatVal += abilityImpact(ability, activeChar, charScript);

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
            
            // Check if there isn't a character on this tile already
            // (not moving gets this bonus, too)
            if (tileScript.hasCharacter == false || tileScript.position == activeTile.position)
            {
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
    }

    // Helper function to determine allies and enemys
    private static bool IsAlly(Character charScript)
    {
        // Compare the alliance of self and other and then return it
        return activeChar.IsPlayer() == charScript.IsPlayer();
    }

    public static TileScript GetBestTile()
    {
        Debug.Log("HeatmapGenerator: the best tile has a score of " + highScore.ToString());

        return bestTile;
    }

    // Get ability impact
    public static int abilityImpact(Ability a, Character caster, Character target)
    {
        int totalImpact = 0;
        int killBonus = 50;

        // Damage
        int damage = a.baseDmg * a.totalHits + caster.atk;
        totalImpact += damage;

        // Healing
        int healing = Mathf.Max((a.baseHp + target.GetLuck()), (target.hpmax.baseValue - target.hp));
        healing *= a.totalHits;
        totalImpact += healing;

        // Would kill bonus
        if (damage > target.hp)
        {
            totalImpact += killBonus;
        }

        // Accuracy bonus
        //totalImpact += a.baseAcc;

        return totalImpact;
    }
}   
