/// @author: Bryson Squibb
/// @date: 10/08/2022
/// @description: this script is responsible for
/// holding the data of tile objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Position
    [SerializeField] public Vector2Int position;

    // Game object references
    public Grid grid;
    public GameObject characterOn = null;
    public GameObject objectOn = null;
    public Grid targetGrid = null;
    public Tile targetTile = null;

    // Flags
    public bool hasCharacter = false;
    public bool hasObject = false;
    public bool highlighted = false;
    public bool passable = true;
    public bool hasGridLink = false;

    // Layer
    public int myLayer;

    // Path to root
    public PathTreeNode PathRef = null;

    // Experimental: heat map
    public int heatVal = 0;
}
