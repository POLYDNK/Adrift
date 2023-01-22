using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Look at camera
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
