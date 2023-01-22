using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTreeNode
{
    // Parent node
    public PathTreeNode parent = null;

    // Cardinal directions
    public PathTreeNode up = null;
    public PathTreeNode down = null;
    public PathTreeNode left = null;
    public PathTreeNode right = null;

    // My tile
    public GameObject myTile = null;

    // Tile range
    public int tileRange;

    // Constructors
    public PathTreeNode() {}
    public PathTreeNode(PathTreeNode p, GameObject t, int range)
    {
        parent = p;
        myTile = t;
        myTile.GetComponent<TileScript>().pathRef = this;
        tileRange = range;
    }

    public PathTreeNode copy() {
        PathTreeNode p = new PathTreeNode();
        if(p.parent != null) p.parent = parent.copy();
        if(p.up != null) p.up = up.copy();
        if(p.down != null) p.down = down.copy();
        if(p.left != null) p.left = left.copy();
        if(p.right != null) p.right = right.copy();
        p.myTile = myTile;
        p.tileRange = tileRange;
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
        while (currentNode.parent != null)
        {
            // Add the current node's parent to path
            path.Push(currentNode.parent);

            //Debug.Log("Adding tile at position " + currentNode.myTile.transform.position.ToString() + " to path");

            // Go to parent
            currentNode = currentNode.parent;
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
        while (currentNode.parent != null)
        {
            // Add the current node's parent to path
            path.Add(currentNode.parent);

            //Debug.Log("Adding tile at position " + currentNode.myTile.transform.position.ToString() + " to path");

            // Go to parent
            currentNode = currentNode.parent;
        }
        
        return path;
    }

    // Puts a path of nodes, to the root, onto a stack
    public void PathToRootOnStack(Stack<PathTreeNode> stack)
    {
        // Set current node to this node
        PathTreeNode currentNode = this;

        // Push Self
        stack.Push(this);

        // Do this until the root is found
        while (currentNode.parent != null)
        {
            // Add the current node's parent to path
            stack.Push(currentNode.parent);

            //Debug.Log("Adding tile at position " + currentNode.myTile.transform.position.ToString() + " to path");

            // Go to parent
            currentNode = currentNode.parent;
        }
    }
}
