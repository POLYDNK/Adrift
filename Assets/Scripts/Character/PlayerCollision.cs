using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] GameObject SplashEffect;
    [SerializeField] CharacterStats CharStats;

    // References
    private Rigidbody rbGO;

    public void sleepRigidbody()
    {
        Destroy(rbGO);
    }

    public void wakeRigidBody()
    {
        rbGO = gameObject.AddComponent<Rigidbody>();
    }   

    // Do this when colliding w/ a ship
    void OnCollisionEnter(Collision other)
    {
        string colTag = other.gameObject.tag;

        if (colTag == "Ship")
        {
            // Get script of the colliding ship
            Transform otherShip = other.gameObject.transform.parent;
            var otherShipStats = otherShip.GetComponent<ShipStats>();

            // Remove self from current ship
            Vector2Int tilePos = transform.parent.transform.GetComponent<CharacterStats>().gridPosition;
            GameObject myTile = transform.parent.transform.GetComponent<CharacterStats>().myGrid.GetComponent<GridBehavior>().GetTileAtPos(tilePos);
            var tileScript = myTile.GetComponent<TileScript>();
            tileScript.characterOn = null;
            tileScript.hasCharacter = false;

            // Get onto ship
            otherShipStats.deckGrid.GetComponent<GridBehavior>().SpawnCharacter(transform.parent.gameObject, false);

            // Update Transformation
            transform.position = transform.parent.transform.position;
            transform.rotation = transform.parent.transform.rotation;

            // Sleep Rigid Body
            sleepRigidbody();            
        }
    }

    // Drowning
    void OnTriggerEnter(Collider other)
    {
        string colTag = other.gameObject.tag;
        
        if (colTag == "Ocean")
        {
            // Play audio
            var audioData = GetComponent<AudioSource>();
            audioData.Play(0);

            // Spawn Splash
            GameObject splash = Instantiate(SplashEffect);
            splash.transform.position = this.transform.position;

            // Deplete HP
            CharStats.adjustHP(-10000, false);
            CharStats.isDead();
        }
    }
}
