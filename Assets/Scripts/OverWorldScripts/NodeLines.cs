using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLines : MonoBehaviour
{
    GameObject NodeParent;
    GameObject child;
    static GameObject[] NodeList;
    static GameObject[][] neighbors;
    static Dictionary<GameObject, int> nodeLookup = new Dictionary<GameObject, int>();
    // Start is called before the first frame update

    void Start()
    {
        NodeParent = this.gameObject;
        NodeList = new GameObject[NodeParent.transform.childCount];
        neighbors = new GameObject[NodeParent.transform.childCount][];
        for(int i = 0; i < NodeParent.transform.childCount; i++)
        {
            NodeList[i] = NodeParent.transform.GetChild(i).gameObject;
            
        }
        for(int i=0; i < NodeParent.transform.childCount; i++)
        {
            nodeLookup.Add(NodeList[i], i);
        }
        for(int i =0; i<neighbors.Length; i++)
        {
            neighbors[i] = new GameObject[NodeList[i].transform.childCount];
        }
        for(int i = 0; i < neighbors.Length; i++)
        {
            for(int k=0; k < neighbors[i].Length; k++)
            {
                neighbors[i][k] = NodeList[i].transform.GetChild(k).gameObject;
            }
        }

//for debugging
        for(int i = 0; i < neighbors.Length; i++)
        {
            for(int k = 0; k < neighbors[i].Length; k++)
           {
                Debug.Log(i + " " + k + " " + neighbors[i][k].name);
            }
        }        
    }

    private void Awake()
    {
        
    }
    //returns neighbors list
    public GameObject[][] getNeighborsList()
    {
        return neighbors;
    }

    public static bool isNeighbor(GameObject currentNode, GameObject neighbor)
    {
        int index;
        nodeLookup.TryGetValue(currentNode.transform.parent.gameObject, out index);
        if (index + 1 < neighbors.Length)
        {
            for (int i = 0; i < neighbors[index + 1].Length; i++)
            {
                if (neighbors[index + 1][i] == neighbor)
                {
                    return true;
                }
            }
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
