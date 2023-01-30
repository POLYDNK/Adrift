using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject boat;
    float cameraDistance = 15f;
    float scale = 0.4f;

    void Start()
    {
        boat = GameObject.Find("Unit");    
    }

    
    
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(boat.transform.position.x, cameraDistance, boat.transform.position.z-5);
        cameraDistance -= Input.mouseScrollDelta.y * scale;
        if(cameraDistance > 25)
        {
            cameraDistance = 25;
        }
        if(cameraDistance < 10)
        {
            cameraDistance = 10;
        }
    }
}
