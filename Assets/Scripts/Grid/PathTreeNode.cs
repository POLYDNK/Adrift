using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTreeNode
{
    // Parent node
    public PathTreeNode Parent = null;

    // Cardinal directions
    public PathTreeNode Up = null;
    public PathTreeNode Down = null;
    public PathTreeNode Left = null;
    public PathTreeNode Right = null;

    // My tile
    public GameObject MyTile = null;

    // Tile range
    public int TileRange;

    // Self and child tiles as a list
    private List<GameObject> allTiles = null;

    // Units that are either on this tile or on child tiles
    private List<Character> allUnits = null;

    // Constructors
    public PathTreeNode() {}
    public PathTreeNode(PathTreeNode p, GameObject t, int range)
    {
        Parent = p;
        MyTile = t;
        MyTile.GetComponent<Tile>().PathRef = this;
        TileRange = range;
    }

    public PathTreeNode Copy() {
        PathTreeNode p = new PathTreeNode();
        if(p.Parent != null) p.Parent = Parent.Copy();
        if(p.Up != null) p.Up = Up.Copy();
        if(p.Down != null) p.Down = Down.Copy();
        if(p.Left != null) p.Left = Left.Copy();
        if(p.Right != null) p.Right = Right.Copy();
        p.MyTile = MyTile;
        p.TileRange = TileRange;
        return p;
    }

    // Get path to root
    public Stack<PathTreeNode> PathToRoot()
    {
        Stack<PathTreeNode> path = new Stack<PathTreeNode>(); // Create path stack
        PathTreeNode currentNode = this; // Set current node to this node

        // Push Self
        path.Push(this);

        // Do this until the root is found
        while (currentNode.Parent != null)
        {
            // Add the current node's parent to path
            path.Push(currentNode.Parent);

            //Debug.Log("Adding tile at position " + currentNode.myTile.transform.position.ToString() + " to path");

            // Go to parent
            currentNode = currentNode.Parent;
        }
        
        return path;
    }

    // Get path to root as a list
    public List<PathTreeNode> PathToRootList()
    {
        List<PathTreeNode> path = new List<PathTreeNode>(); // Create path list
        PathTreeNode currentNode = this; // Set current node to this node

        // Push Self
        path.Add(this);

        // Do this until the root is found
        while (currentNode.Parent != null)
        {
            // Add the current node's parent to path
            path.Add(currentNode.Parent);

            //Debug.Log("Adding tile at position " + currentNode.myTile.transform.position.ToString() + " to path");

            // Go to parent
            currentNode = currentNode.Parent;
        }
        
        return path;
    }

    // Get all tiles below this node, including this one, as a list
    public List<GameObject> GetAllTiles()
    {
        // Create a new list
        allTiles = new List<GameObject>();

        // Call a helper function to populate the list
        PathTreePreOrderTiles(this);

        // Return the populated list
        return allTiles;
    }

    // Get all units below this node, including this one, as a list
    public List<Character> GetAllUnits()
    {
        // Create a new list
        allUnits = new List<Character>();

        // Call a helper function to populate the list
        PathTreePreOrderUnits(this);

        // Return the populated list
        return allUnits;
    }

    // Helper recursive function to populate the all tiles list
    private void PathTreePreOrderTiles(PathTreeNode currNode)
    {
        // Put the tile object of the current node onto the list
        if (allTiles.Contains(currNode.MyTile) == false)
        {
            allTiles.Add(currNode.MyTile);
        }

        // Visit the Up, Down, Left, and Right nodes if able
        if (currNode.Up != null)    { PathTreePreOrderTiles(currNode.Up);    }
        if (currNode.Down != null)  { PathTreePreOrderTiles(currNode.Down);  }
        if (currNode.Left != null)  { PathTreePreOrderTiles(currNode.Left);  }
        if (currNode.Right != null) { PathTreePreOrderTiles(currNode.Right); }
    }

    // Helper recursive function to populate the all units list
    private void PathTreePreOrderUnits(PathTreeNode currNode)
    {
        Character charOnTile = null;
        Tile myTileScript = currNode.MyTile.GetComponent<Tile>();

        // Get character on tile if able
        if (myTileScript.characterOn != null)
        {
            charOnTile = myTileScript.characterOn.GetComponent<Character>();
        }

        // Add the character to the list if it's not
        // already in it
        if (allUnits.Contains(charOnTile) == false)
        {
            allUnits.Add(charOnTile);
        }

        // Visit the Up, Down, Left, and Right nodes if able
        if (currNode.Up != null)    { PathTreePreOrderUnits(currNode.Up);    }
        if (currNode.Down != null)  { PathTreePreOrderUnits(currNode.Down);  }
        if (currNode.Left != null)  { PathTreePreOrderUnits(currNode.Left);  }
        if (currNode.Right != null) { PathTreePreOrderUnits(currNode.Right); }
    }

    // Puts a path of nodes, to the root, onto a stack
    public void PathToRootOnStack(Stack<PathTreeNode> stack)
    {
        // Set current node to this node
        PathTreeNode currentNode = this;

        // Push Self
        stack.Push(this);

        // Do this until the root is found
        while (currentNode.Parent != null)
        {
            // Add the current node's parent to path
            stack.Push(currentNode.Parent);

            //Debug.Log("Adding tile at position " + currentNode.myTile.transform.position.ToString() + " to path");

            // Go to parent
            currentNode = currentNode.Parent;
        }
    }
}
