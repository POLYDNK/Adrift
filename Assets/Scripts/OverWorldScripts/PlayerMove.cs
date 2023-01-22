using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameObject BOAT;
    public float zAxis;
    public float xAxis;
    public float yAxis;
    public float originalYAxis=1;
    public bool FloatUp = true;
    public static GameObject destination;
    public static GameObject destination2;
    public bool travel = false;
    public static GameObject currNode;//=GameObject.Find("StartNode");
    public static bool displayTextBox;

    private void Awake()
    {
        currNode = GameObject.Find("StartNode");

    }
    // Start is called before the first frame update
    void Start()
    {

        originalYAxis = 1.5f;
        BOAT = this.gameObject;
        zAxis = currNode.transform.position.z;//BOAT.transform.position.z;
        xAxis = currNode.transform.position.x;
        yAxis = originalYAxis;
        //originalYAxis = yAxis;
        BOAT.transform.position = new Vector3(xAxis, originalYAxis, zAxis);
        StartCoroutine(floating());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(floating());
        BOAT.transform.position = new Vector3(xAxis, yAxis, zAxis);
        if (Input.GetMouseButtonDown(0))
        {
            displayTextBox = true;
            if (travel != true)
            {
                if (destination != null)
                {
                    destination2 = NodeClick.GetClickObj();
                    if (destination == destination2)
                    {
                        if (NodeLines.isNeighbor(currNode, destination2))
                        {
                            travel = true;
                            currNode = destination2;
                            
                            StartCoroutine(Sail());
                        }
                    }
                    else
                    {
                        NodeInfo.hideTextBox();
                        travel = false;
                    }
                }
                destination = NodeClick.GetClickObj();
                
            }
        }
    }

    public IEnumerator Sail()
    {
        float distanceX = xAxis-destination.transform.position.x;
        float distanceZ = zAxis-destination.transform.position.z;        
        float velocity = 3f;
        Vector3 look = new Vector3(destination.transform.position.x, BOAT.transform.position.y, destination.transform.position.z);
        displayTextBox = false;
        NodeInfo.hideTextBox();
        while (travel == true)
        {
            yield return new WaitForSeconds(0.0f);
            BOAT.transform.LookAt(look);
            BOAT.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            
            if (distanceX < -0.05f && xAxis < destination.transform.position.x) 
            { 
                xAxis += velocity*Time.deltaTime; 
            }
            if (distanceX > 0.05f && xAxis > destination.transform.position.x)
            {
                xAxis -= velocity * Time.deltaTime;
            }
            if (distanceZ < -0.05f && zAxis < destination.transform.position.z)
            {
                zAxis += velocity *Time.deltaTime;
            }
            if (distanceZ > 0.05f && zAxis > destination.transform.position.z)
            {
                zAxis -= velocity * Time.deltaTime;
            }
            if(distanceZ < 0.05f && distanceZ > -0.05f && distanceX <0.05f && distanceX > -0.05f)
            {
            zAxis = destination.transform.position.z;
            xAxis = destination.transform.position.x;
            travel = false;
            }
            distanceX = xAxis - destination.transform.position.x;
            distanceZ = zAxis - destination.transform.position.z;

        }
        NodeInfo.loadScene();
    }

    public IEnumerator floating()
    {
        yield return new WaitForSeconds(0.5f);
        if (FloatUp == true)
        {
            yAxis += (0.25f * Time.deltaTime);
            if (yAxis > (originalYAxis + 0.25f))
            {

                FloatUp = false;
            }
        }
        if (FloatUp == false)
        {
            yAxis -= (0.25f * Time.deltaTime);
            if (yAxis < (originalYAxis - .25f))
            {               
                FloatUp = true;
            }
        }
    }
    public static GameObject getDestination()
    {
        return destination;
    }
    public static GameObject getCurrNode()
    {
        return currNode;
    }
}
