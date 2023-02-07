using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterCollision : MonoBehaviour
{
    [FormerlySerializedAs("SplashEffect")] [SerializeField] GameObject splashEffect;
    [FormerlySerializedAs("CharStats")] [SerializeField] Character charStats;

    // References
    private Rigidbody rbGo;

    public void SleepRigidbody()
    {
        Destroy(rbGo);
    }

    public void WakeRigidBody()
    {
        rbGo = gameObject.AddComponent<Rigidbody>();
    }   

    // Do this when colliding w/ a ship
    void OnCollisionEnter(Collision other)
    {
        string colTag = other.gameObject.tag;

        if (colTag == "Ship")
        {
            // Get script of the colliding ship
            Transform otherShip = other.gameObject.transform.parent;
            var otherShipStats = otherShip.GetComponent<Ship>();

            // Remove self from current ship
            Vector2Int tilePos = transform.parent.transform.GetComponent<Character>().gridPosition;
            GameObject myTile = transform.parent.transform.GetComponent<Character>().myGrid.GetComponent<Grid>().GetTileAtPos(tilePos);
            var tileScript = myTile.GetComponent<Tile>();
            tileScript.characterOn = null;
            tileScript.hasCharacter = false;

            // Get onto ship
            otherShipStats.deckGrid.GetComponent<Grid>().SpawnCharacter(transform.parent.gameObject, false);

            // Update Transformation
            transform.position = transform.parent.transform.position;
            transform.rotation = transform.parent.transform.rotation;

            // Sleep Rigid Body
            SleepRigidbody();            
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
            GameObject splash = Instantiate(splashEffect);
            splash.transform.position = this.transform.position;

            // Deplete HP
            charStats.AdjustHp(-10000, false);
            charStats.IsDead();
        }
    }
}
