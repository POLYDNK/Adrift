using System;
using UnityEngine;

public class TextFloat : MonoBehaviour
{
    // Public vars
    [SerializeField] public Vector3 floatDistance;
    [SerializeField] public float floatTime;
    [SerializeField] public GameObject objFollow;

    // Private vars
    private float xDrift;
    private float yDrift;
    private float zDrift;
    private float percent;

    // Start is called before the first frame update
    void Start()
    {
        // Randomized Drift Distance
        xDrift = (UnityEngine.Random.value * floatDistance.x) - floatDistance.x/2f;
        yDrift = (UnityEngine.Random.value * floatDistance.y) - floatDistance.y/2f;
        zDrift = (UnityEngine.Random.value * floatDistance.z) - floatDistance.z/2f;

        // Percent of Drift Left to Complete
        percent = 1.00f;
    }

    // Update is called once per frame
    void Update()
    {
        //percent -= Time.deltaTime;
        percent = Math.Max(0.0f, percent - Time.deltaTime / floatTime);

        Vector3 currOffset = new Vector3((xDrift * percent),
                                         (yDrift * percent),
                                         (zDrift * percent));

        // Update text pos
        transform.position = Camera.main.WorldToScreenPoint(objFollow.transform.position + currOffset);

    }
}
