using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public int gridSelected = 0;

    // ----- Default Grid -----
    public const uint DefaultGridWidth = 8;
    public const uint DefaultGridHeight = 16;
    public static bool[,] DefaultGridPassableFlags = 
    {
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},   
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false}
    };
    // ------------------------

    // ------- Grid One -------
    public const uint GridOneWidth = 9;
    public const uint GridOneHeight = 20;
    public static bool[,] GridOnePassableFlags = 
    {
        {false, false, false, false, false, false, false, false, false},
        {true , false, false, false, false, false, false, false, true },
        {false, false, false, false, false, false, false, false, false},
        {false, false, true , true , true , true , true , false, false},
        {true , false, true , true , true , true , true , false, true },
        {false, false, true , true , true , true , true , false, false},
        {false, false, true , true , true , true , true , false, false},
        {true , false, true , true , true , true , true , false, true },
        {false, false, true , true , true , true , true , false, false},
        {false, false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false, false},
        {false, false, true , true , true , true , true , false, false},
        {false, false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false, false},
        {false, false, false, true , true , true , false, false, false},
        {false, false, false, true , true , true , false, false, false},
        {false, false, false, true , true , true , false, false, false},
        {true , false, false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, false, true }
    };

    public static float[,] GridOneHeightOffsets = 
    {
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f},
        {-1.8f, -1.8f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -1.8f, -1.8f},
        {-0.6f, -0.6f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -0.6f, -0.6f},
        {-0.2f, -0.2f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f},
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f}
    };
    // ------------------------

    // ------- Grid Two -------
    public const uint GridTwoWidth = 8;
    public const uint GridTwoHeight = 39;
    public static bool[,] GridTwoPassableFlags = 
    {
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, true , true , true , true , false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {true , false, false, true , true , false, false, true },
        {false, false, false, false, false, false, false, false},
        {true , false, false, false, false, false, false, true },
        {false, false, false, true , true , false, false, false},
        {true , false, false, true , true , false, false, true },
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {true , false, false, false, false, false, false, true },
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, true , true , true , true , false, false},
        {false, false, true , true , true , true , false, false},
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , true , false, false, false, false, true , true },
        {true , true , false, false, false, false, true , true },
        {true , true , true , false, false, true , true , true },
        {true , true , true , false, false, true , true , true }

    };

    public static float[,] GridTwoHeightOffsets = 
    {
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f, -0.2f },
        {-1.0f, -1.0f, -2.3f, -2.3f, -2.3f, -2.3f, -1.0f, -1.0f },
        {-1.5f, -1.5f, -2.3f, -2.3f, -2.3f, -2.3f, -1.5f, -1.5f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f, -2.3f },
        {-1.8f, -1.8f, -2.3f, -2.3f, -2.3f, -2.3f, -1.8f, -1.8f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f },
        {-1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f, -1.2f }
    };

    // ------------------------

    // ------- Grid Three -------
    public const uint GridThreeWidth = 8;
    public const uint GridThreeHeight = 20;
    public static bool[,] GridThreePassableFlags = 
    {
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},

    };

    public static float[,] GridThreeHeightOffsets = 
    {
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        { 0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
    };

    // ------------------------

    // Constructors
    public GridMap() {}
    public GridMap(int gridNumber) {gridSelected = gridNumber;}

    // Select a grid map
    public void SelectGrid(int gridNumber)
    {
        switch (gridNumber)
        {
            case 0:
                gridSelected = 0;
                break;
            case 1:
                gridSelected = 1;
                break;
            case 2:
                gridSelected = 2;
                break;
            case 3:
                gridSelected = 3;
                break;
            default:
                Debug.Log("SelectGrid(): Error! Grid number is out of range. Selecting default grid instead");
                gridSelected = 0;
                break;
        }
    }

    // Get the width of a grid map
    public uint GetGridWidth()
    {  
        uint widthToReturn = 0;

        switch(gridSelected)
        {
            case 0:
                widthToReturn = DefaultGridWidth;
                break;
            case 1:
                widthToReturn = GridOneWidth;
                break;
            case 2:
                widthToReturn = GridTwoWidth;
                break;
            case 3:
                widthToReturn = GridThreeWidth;
                break;
            default:
                Debug.Log("GetGridWidth(): Error! Cannot find selected grid");
                break;
        }

        return widthToReturn;
    }

    // Get the height of a grid map
    public uint GetGridHeight()
    {  
        uint heightToReturn = 0;

        switch(gridSelected)
        {
            case 0:
                heightToReturn = DefaultGridHeight;
                break;
            case 1:
                heightToReturn = GridOneHeight;
                break;
            case 2:
                heightToReturn = GridTwoHeight;
                break;
            case 3:
                heightToReturn = GridThreeHeight;
                break;
            default:
                Debug.Log("GetGridHeight(): Error! Cannot find selected grid");
                break;
        }

        return heightToReturn;
    }

    // Get the flag at a given grid map position
    public bool GetFlagAtPos(Vector2Int pos)
    {
        bool flagAtPos = false;

        switch(gridSelected)
        {
            case 0:
                flagAtPos = DefaultGridPassableFlags[pos.y, pos.x];
                break;
            case 1:
                flagAtPos = GridOnePassableFlags[pos.y, pos.x];
                break;
            case 2:
                flagAtPos = GridTwoPassableFlags[pos.y, pos.x];
                break;
            case 3:
                flagAtPos = GridThreePassableFlags[pos.y, pos.x];
                break;
            default:
                Debug.Log("GetFlagAtPos(): Error! Cannot find selected grid");
                break;
        }

        return flagAtPos;
    }

    // Get the flag at a given grid map position
    public float GetHeightOffsetAtPos(Vector2Int pos)
    {
        float offsetAtPos = 0.0f;

        switch(gridSelected)
        {
            case 0:
                offsetAtPos = 0.0f;
                break;
            case 1:
                offsetAtPos = GridOneHeightOffsets[pos.y, pos.x];
                break;
            case 2:
                offsetAtPos = GridTwoHeightOffsets[pos.y, pos.x];
                break;
            case 3:
                offsetAtPos = GridThreeHeightOffsets[pos.y, pos.x];
                break;
            default:
                Debug.Log("GetHeightOffsetAtPos(): Error! Cannot find selected grid");
                break;
        }

        return offsetAtPos;
    }
}
