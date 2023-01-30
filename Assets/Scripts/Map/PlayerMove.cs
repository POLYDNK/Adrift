using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [FormerlySerializedAs("BOAT")] public GameObject boat;
    public float zAxis;
    public float xAxis;
    public float yAxis;
    public float originalYAxis=1;
    [FormerlySerializedAs("FloatUp")] public bool floatUp = true;
    public static GameObject Destination;
    public static GameObject Destination2;
    public bool travel = false;
    public static GameObject CurrNode;//=GameObject.Find("StartNode");
    public static bool DisplayTextBox;

    private void Awake()
    {
        CurrNode = GameObject.Find("StartNode");

    }
    // Start is called before the first frame update
    void Start()
    {

        originalYAxis = 1.5f;
        boat = this.gameObject;
        zAxis = CurrNode.transform.position.z;//BOAT.transform.position.z;
        xAxis = CurrNode.transform.position.x;
        yAxis = originalYAxis;
        //originalYAxis = yAxis;
        boat.transform.position = new Vector3(xAxis, originalYAxis, zAxis);
        StartCoroutine(Floating());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Floating());
        boat.transform.position = new Vector3(xAxis, yAxis, zAxis);
        if (Input.GetMouseButtonDown(0))
        {
            DisplayTextBox = true;
            if (travel != true)
            {
                if (Destination != null)
                {
                    Destination2 = NodeClick.GetClickObj();
                    if (Destination == Destination2)
                    {
                        if (NodeLines.IsNeighbor(CurrNode, Destination2))
                        {
                            travel = true;
                            CurrNode = Destination2;
                            
                            StartCoroutine(Sail());
                        }
                    }
                    else
                    {
                        NodeInfo.HideTextBox();
                        travel = false;
                    }
                }
                Destination = NodeClick.GetClickObj();
                
            }
        }
    }

    public IEnumerator Sail()
    {
        float distanceX = xAxis-Destination.transform.position.x;
        float distanceZ = zAxis-Destination.transform.position.z;        
        float velocity = 3f;
        Vector3 look = new Vector3(Destination.transform.position.x, boat.transform.position.y, Destination.transform.position.z);
        DisplayTextBox = false;
        NodeInfo.HideTextBox();
        while (travel == true)
        {
            yield return new WaitForSeconds(0.0f);
            boat.transform.LookAt(look);
            boat.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            
            if (distanceX < -0.05f && xAxis < Destination.transform.position.x) 
            { 
                xAxis += velocity*Time.deltaTime; 
            }
            if (distanceX > 0.05f && xAxis > Destination.transform.position.x)
            {
                xAxis -= velocity * Time.deltaTime;
            }
            if (distanceZ < -0.05f && zAxis < Destination.transform.position.z)
            {
                zAxis += velocity *Time.deltaTime;
            }
            if (distanceZ > 0.05f && zAxis > Destination.transform.position.z)
            {
                zAxis -= velocity * Time.deltaTime;
            }
            if(distanceZ < 0.05f && distanceZ > -0.05f && distanceX <0.05f && distanceX > -0.05f)
            {
            zAxis = Destination.transform.position.z;
            xAxis = Destination.transform.position.x;
            travel = false;
            }
            distanceX = xAxis - Destination.transform.position.x;
            distanceZ = zAxis - Destination.transform.position.z;

        }
        NodeInfo.LoadScene();
    }

    public IEnumerator Floating()
    {
        yield return new WaitForSeconds(0.5f);
        if (floatUp == true)
        {
            yAxis += (0.25f * Time.deltaTime);
            if (yAxis > (originalYAxis + 0.25f))
            {

                floatUp = false;
            }
        }
        if (floatUp == false)
        {
            yAxis -= (0.25f * Time.deltaTime);
            if (yAxis < (originalYAxis - .25f))
            {               
                floatUp = true;
            }
        }
    }
    public static GameObject GetDestination()
    {
        return Destination;
    }
    public static GameObject GetCurrNode()
    {
        return CurrNode;
    }
}
