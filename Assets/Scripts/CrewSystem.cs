using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/*
 * Crew System: Ties characters together. Requires 
 * an inventory to manage resources. 
 * Keeps track of 
 * morale and reputation (these both affect encounters). 
 * Should handle anything ship related (currently just health). Focus on programming & communication skills.
 * */


public class CrewSystem : MonoBehaviour
{
    public bool isPlayer; //Whether this crew is a player crew (do not change this in code)
    public List<GameObject> characters; //List of all characters in this crew
    public GameObject ship; //Ship belonging to this crew
    public int morale = 100;

    public int inventory_gold =1 ;
    public int inventory_wood =2 ;
    public int inventory_crew =3 ;
    public int inventory_weapons =4 ;

    public int[] inventory;
    int inventorySize = 19;
    int inventoryItemsIn = 0;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new int[20];
        Object[] allObjects = Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            Debug.Log(go + " is an active object " + go.GetInstanceID());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("works ! ");
            if (true)
            {

            }
        }
        addItemToInventory(inventory_gold);

    }

    public void initCharacters() {
        foreach(GameObject character in characters) {
            character.GetComponent<CharacterStats>().crew = this.gameObject; //Set reference to crew in character
        }
    }

    public ShipStats getShip() {
        return ship.GetComponent<ShipStats>();
    }

    public void addMorale(int morale) {
        this.morale = (int) Mathf.Clamp(this.morale + morale, 0, 100);
    }

    public void addModifier(StatModifier modifier) {
        foreach(GameObject character in characters) {
            character.GetComponent<CharacterStats>().addModifier(modifier);
        }
    }

    public void clearModifiersWithId(string id) {
        foreach(GameObject character in characters) {
            character.GetComponent<CharacterStats>().clearModifiersWithId(id);
        }
    }

    /// <summary>
    /// Function to update an item in the inventory by a given quantity
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="quantity"></param>
    void updateInventory(int itemId, int quantity)
    {
        for (int i = 0; i < inventoryItemsIn; i++)
        {
            if (inventory[i] == itemId)
            {
                inventory[i] *= 2;
            }
        }
    }


    void addItemToInventory(int itemId)
    {
        if (inventoryItemsIn == inventorySize)
        {
            return;
        }
        else
        {
            inventory[inventoryItemsIn] = itemId;
            inventoryItemsIn++;
        }
    }

    void removeItemFromInventory(int itemId)
    {
        for (int i = 0; i < inventoryItemsIn; i++)
        {
            if (inventory[i] == itemId)
            {
                inventory[i] = 0;
                inventoryItemsIn--;
                break;
            }
        }
    }
}
