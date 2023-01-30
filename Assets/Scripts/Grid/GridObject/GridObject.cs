using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField]
    public GameObject gridReference;
    public Vector2Int gridPosition;
    public bool isPlayerOwned;
    public bool overlapCharacter;
    public InteractionType interactionType;
    public string displayNamePrimary, displayNameSecondary;

    private bool isOnGrid = false;

    public enum InteractionType
    {
        None, Single, Double
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnGrid == false && gridReference != null)
        {
            gridReference.GetComponent<Grid>().AttachObjectToGrid(this.gameObject, gridPosition);
            isOnGrid = true;
        }
    }

    // Trigger this object's primary behavior when interacted with by a character, return created object to track with camera
    public GameObject InteractPrimary(GameObject user)
    {
        Debug.Log("Empty primary");
        return null;
    }

    // Trigger this object's secondary behavior when interacted with by a character, return created object to track with camera
    public GameObject InteractSecondary(GameObject user)
    {
        Debug.Log("Empty secondary");
        return null;
    }
}