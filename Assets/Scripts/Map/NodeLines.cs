using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLines : MonoBehaviour
{
    GameObject nodeParent;
    GameObject child;
    static GameObject[] nodeList;
    static GameObject[][] neighbors;
    static Dictionary<GameObject, int> nodeLookup = new Dictionary<GameObject, int>();
    // Start is called before the first frame update

    void Start()
    {
        nodeParent = this.gameObject;
        nodeList = new GameObject[nodeParent.transform.childCount];
        neighbors = new GameObject[nodeParent.transform.childCount][];
        for(int i = 0; i < nodeParent.transform.childCount; i++)
        {
            nodeList[i] = nodeParent.transform.GetChild(i).gameObject;
            
        }
        for(int i=0; i < nodeParent.transform.childCount; i++)
        {
            nodeLookup.Add(nodeList[i], i);
        }
        for(int i =0; i<neighbors.Length; i++)
        {
            neighbors[i] = new GameObject[nodeList[i].transform.childCount];
        }
        for(int i = 0; i < neighbors.Length; i++)
        {
            for(int k=0; k < neighbors[i].Length; k++)
            {
                neighbors[i][k] = nodeList[i].transform.GetChild(k).gameObject;
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
    public GameObject[][] GetNeighborsList()
    {
        return neighbors;
    }

    public static bool IsNeighbor(GameObject currentNode, GameObject neighbor)
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
