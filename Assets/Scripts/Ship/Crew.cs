using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


/*
 * Crew System: Ties characters together. Requires 
 * an inventory to manage resources. 
 * Keeps track of 
 * morale and reputation (these both affect encounters). 
 * Should handle anything ship related (currently just health). Focus on programming & communication skills.
 * */


public class Crew : MonoBehaviour
{
    public bool isPlayer; //Whether this crew is a player crew (do not change this in code)
    public List<GameObject> characters; //List of all characters in this crew
    public Ship ship; //Ship belonging to this crew
    public int morale = 100;

    [FormerlySerializedAs("inventory_gold")] public int inventoryGold =1 ;
    [FormerlySerializedAs("inventory_wood")] public int inventoryWood =2 ;
    [FormerlySerializedAs("inventory_crew")] public int inventoryCrew =3 ;
    [FormerlySerializedAs("inventory_weapons")] public int inventoryWeapons =4 ;

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
        AddItemToInventory(inventoryGold);

    }

    public void InitCharacters() {
        foreach(GameObject character in characters) {
            character.GetComponent<Character>().crew = this.gameObject; //Set reference to crew in character
        }
    }

    public Ship GetShip() {
        return ship.GetComponent<Ship>();
    }

    public void AddMorale(int morale) {
        this.morale = (int) Mathf.Clamp(this.morale + morale, 0, 100);
    }

    public void AddModifier(StatModifier modifier) {
        foreach(GameObject character in characters) {
            character.GetComponent<Character>().AddModifier(modifier);
        }
    }

    public void ClearModifiersWithId(string id) {
        foreach(GameObject character in characters) {
            character.GetComponent<Character>().ClearModifiersWithId(id);
        }
    }

    /// <summary>
    /// Function to update an item in the inventory by a given quantity
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="quantity"></param>
    void UpdateInventory(int itemId, int quantity)
    {
        for (int i = 0; i < inventoryItemsIn; i++)
        {
            if (inventory[i] == itemId)
            {
                inventory[i] *= 2;
            }
        }
    }


    void AddItemToInventory(int itemId)
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

    void RemoveItemFromInventory(int itemId)
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
