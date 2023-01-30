using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Ship : MonoBehaviour
{
    [FormerlySerializedAs("HP")] [SerializeField] public int hp;    // Maximum health
    [FormerlySerializedAs("HPMAX")] [SerializeField] public int hpmax; // Maximum health
    [SerializeField] public bool isEnemy;

    [SerializeField] public Animation shipSinkAnim;
    [SerializeField] public ShipController myMoveScript;

    [SerializeField] public GameObject waterCollider;
    [SerializeField] public GameObject foamCollider;

    [SerializeField] public GameObject deckGrid;

    [SerializeField] public float sinkTiltAmount;
    [SerializeField] public float sinkTiltSpeed;

    private bool isSinking = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set HP
        hp = hpmax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // HP changed (either by taking damage (negative) or repairing (positive))
    public void AdjustHp(int change)
    {
        // Adjust current health
        hp += change;

        // Sink if HP is depleted
        if(hp <= 0 && isSinking == false)
        {
            hp = 0;
            Sink();
        }

        // Prevent overhealing
        if(hp > hpmax)
        {
            hp = hpmax;
        }
    }

    // Sink ship
    private void Sink()
    {
        // Create water collider
        GameObject myWaterCollider = GameObject.Instantiate(waterCollider, transform);

        // Destroy foam collider
        GameObject.Destroy(foamCollider);

        // Animation
        shipSinkAnim.Play();
        myMoveScript.tiltAmount = sinkTiltAmount;
        myMoveScript.tiltSpeed = sinkTiltSpeed;

        // Audio audio
        var audioData = GetComponent<AudioSource>();
        audioData.Play(0);

        // Flags
        isSinking = true;   
    }
}
