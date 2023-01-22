using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXPlayRate : MonoBehaviour
{
    private VisualEffect visualEffect;
    [SerializeField] public float PlayRate;

    // Set Play Rate
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        visualEffect.playRate = PlayRate;
    }
}
