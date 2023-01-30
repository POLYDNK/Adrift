using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Destroy Self
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
