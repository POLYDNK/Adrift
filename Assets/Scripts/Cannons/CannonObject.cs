using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonObject : GridObject
{
    // Values
    [SerializeField] public float shotSpeed;
    [SerializeField] public int   shotDamage;
    [SerializeField] public float shotRandomness;

    // Assets
    public GameObject cannonball;
    public GameObject muzzleFlash;

    // Muzzle Object
    public Transform muzzle;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void fireEffect()
    {
         // ----- Audio -----
        var audioData = GetComponent<AudioSource>();
        audioData.Play(0);

        // ----- Muzzle Flash -----
        GameObject mFlash = Instantiate(muzzleFlash);
        mFlash.transform.position = muzzle.position;
    }

    public GameObject fireCannonball(bool fromEnemy)
    {
        // ----- Cannonball Spawning -----
        GameObject newCannonball = Instantiate(cannonball);
        newCannonball.GetComponent<CannonballCollision>().damage = shotDamage;
        newCannonball.GetComponent<CannonballCollision>().isEnemyOwned = fromEnemy;
        return fireObject(newCannonball);
    }

    public GameObject fireObject(GameObject obj)
    {
        fireEffect();

        // ----- Object Positioning -----
        obj.transform.position = muzzle.position;
        obj.transform.rotation = muzzle.rotation;
        var rb = obj.GetComponent<Rigidbody>();

        // Calculate Object velocity
        Vector3 shotVelocity = muzzle.forward * shotSpeed;
        shotVelocity.x += (Random.value * shotRandomness) - shotRandomness/2;
        shotVelocity.y += (Random.value * shotRandomness) - shotRandomness/2;
        shotVelocity.z += (Random.value * shotRandomness) - shotRandomness/2;

        // Apply calculated velocity
        rb.velocity = shotVelocity;
        return obj;
    }

    // Trigger this object's primary behavior when interacted with by a character, return created object to track with camera
    new public GameObject InteractPrimary(GameObject user)
    {
        Debug.Log("Fire Cannonball!");
        return fireCannonball(!user.GetComponent<CharacterStats>().isPlayer());
    }

    // Trigger this object's secondary behavior when interacted with by a character, return created object to track with camera
    new public GameObject InteractSecondary(GameObject user)
    {
        Debug.Log("Fire User!");
        var rigidBody = user.transform.GetChild(0);
        var script = rigidBody.GetComponent<PlayerCollision>();
        script.wakeRigidBody();
        return fireObject(rigidBody.gameObject);
    }
}
