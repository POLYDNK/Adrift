using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiminishingLight : MonoBehaviour
{
    [SerializeField] public Light lightSource;
    [SerializeField] public float lightMax;
    [SerializeField] public float lightDiminish;

    // Start is called before the first frame update
    void Start()
    {
        lightSource.intensity = lightMax;
        lightSource.range = 10;
    }

    // Update is called once per frame
    void Update()
    {
        lightSource.intensity -= lightDiminish * Time.deltaTime;
        lightSource.range -= 100f * Time.deltaTime;
    }
}
