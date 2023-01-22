using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStats : MonoBehaviour
{
    [SerializeField] public int HP;    // Maximum health
    [SerializeField] public int HPMAX; // Maximum health
    [SerializeField] public bool isEnemy;

    [SerializeField] public Animation shipSinkAnim;
    [SerializeField] public ShipMove myMoveScript;

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
        HP = HPMAX;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // HP changed (either by taking damage (negative) or repairing (positive))
    public void adjustHP(int change)
    {
        // Adjust current health
        HP += change;

        // Sink if HP is depleted
        if(HP <= 0 && isSinking == false)
        {
            HP = 0;
            Sink();
        }

        // Prevent overhealing
        if(HP > HPMAX)
        {
            HP = HPMAX;
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
