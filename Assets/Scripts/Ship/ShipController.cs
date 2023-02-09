using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    [SerializeField] public Vector3 moveDirection;

    [SerializeField] public float tiltSpeed;
    [SerializeField] public float tiltAmount;
    [SerializeField] public Vector3 tiltDirection;

    private float tiltCounter;
    private Vector3 tiltOrigin = new(0.0f, 0.0f, 0.0f);

    void Start()
    {
        // Normalize move direction and tilt direction
        moveDirection.Normalize();
        tiltDirection.Normalize();
    }
    
    void Update()
    {
        // Move in direction set by move direction
        if(moveSpeed > 0F) transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Tilting
        Vector3 newTilt = tiltOrigin + (tiltDirection * Mathf.Sin(tiltCounter) * tiltAmount / 2);
        transform.localRotation = Quaternion.Euler(newTilt.x, newTilt.y, newTilt.z);

        // Update tilt counter
        tiltCounter += Time.deltaTime * tiltSpeed / 10;
    }
}
