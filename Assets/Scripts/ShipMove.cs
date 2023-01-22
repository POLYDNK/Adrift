using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMove : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    [SerializeField] public Vector3 moveDirection;

    [SerializeField] public Quaternion spawnRotation;

    [SerializeField] public float tiltSpeed;
    [SerializeField] public float tiltAmount;
    [SerializeField] public Vector3 tiltDirection;

    private float tiltCounter = 0.0f;
    private Vector3 tiltOrigin = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        // Normalize move direction and tilt direction
        moveDirection.Normalize();
        tiltDirection.Normalize();

        // Update Rotation
        transform.rotation = spawnRotation;
    }
    void Update()
    {
        // Move in direction set by move direction
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Tilting
        Vector3 newTilt = tiltOrigin + (tiltDirection * Mathf.Sin(tiltCounter) * tiltAmount / 12);
        transform.rotation = new Quaternion(newTilt.x, newTilt.y, newTilt.z, 10.0f);

        // Update tilt counter
        tiltCounter += Time.deltaTime * tiltSpeed / 10;
    }
}
