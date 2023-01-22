using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Control Speed
    [SerializeField] public float ControlSpeed, PanSpeed; // WASD Speed and Pan Speed
    [SerializeField] public float MinHeight, MaxHeight; // Min and Max Heights

    // Viewing offset
    private Vector3 offset;

    // Position or object camera is looking at
    [SerializeField] public GameObject objectFollowing = null;
    private Vector3 target;

    // If this camera is being controlled by the player
    private bool controlling = false;
    private bool travelingToTarget = false;

    // Layer modes
    public enum LAYERMODE
    {
        SAILS = 7,
        HELM = 8,
        DECK = 9,
        CABIN = 10
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set offset to it's starting position
        offset = transform.position;
        target = offset;

        //SetLayerMode(LAYERMODE.HELM);
    }
    
    void Update()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        if (xDirection != 0 || zDirection != 0)
        {
            Vector3 moveVector = new Vector3(xDirection, 0.0f, zDirection);
            transform.position += moveVector * Time.deltaTime * ControlSpeed;
            target = transform.position;

            objectFollowing = null;
            controlling = true;
        }
        else
        {
            controlling = false;
        }

        // Zooming
        if (Input.mouseScrollDelta.y > 0.1f || Input.mouseScrollDelta.y < -0.1f)
        {
            objectFollowing = null;
            controlling = true;
            this.transform.Translate(Vector3.forward * Input.mouseScrollDelta.y, this.transform);
        }

        // Layer Setting
        if (Input.GetKey(KeyCode.Keypad0))
        {
            SetLayerMode(LAYERMODE.SAILS);
        }

        if (Input.GetKey(KeyCode.Keypad1))
        {
            SetLayerMode(LAYERMODE.HELM);
        }

        if (Input.GetKey(KeyCode.Keypad2))
        {
            SetLayerMode(LAYERMODE.DECK);
        }

        if (Input.GetKey(KeyCode.Keypad3))
        {
            SetLayerMode(LAYERMODE.CABIN);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // If following an object
        if (objectFollowing != null)
        {
            target = objectFollowing.transform.position + offset;
            travelingToTarget = true;
        }

        // Travel to target
        if (controlling == false && travelingToTarget == true)
        {
            float distToTarget = Vector3.Distance(transform.position, target);
            float moveDist = Vector3.Distance(new Vector3(0,0,0), (target - transform.position) * Time.deltaTime * PanSpeed);

            if (distToTarget <= moveDist)
            {
                transform.position = target;
                travelingToTarget = false;
            }
            else
            {
                transform.position += (target - transform.position) * Time.deltaTime * PanSpeed;
            }
        }

        // Clamping
        float newY = Mathf.Clamp(transform.position.y, MinHeight, MaxHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // Look at a position by setting the target
    public void LookAtPos(Vector3 pos)
    {
        target = pos + offset;
        travelingToTarget = true;
        objectFollowing = null;
    }

    public void SetCameraFollow(GameObject objectToFollow)
    {
        objectFollowing = objectToFollow;
    }

    public void StopCameraFollow()
    {
        objectFollowing = null;
    }

    public void SetLayerMode(LAYERMODE lm)
    {
        // Vars
        int layerMask = 0;

        // Add base layers to LayerMask
        layerMask |= (1 << 0); // Default
        layerMask |= (1 << 1); // TransparentFX
        layerMask |= (1 << 2); // Ignore Raycast
        layerMask |= (1 << 4); // Water
        layerMask |= (1 << 5); // UI
        layerMask |= (1 << 6); // GRID

        // Apply additional layers based on mode
        switch(lm)
        {
            case LAYERMODE.SAILS:
                layerMask |= (1 << 7); // Sails
                goto case LAYERMODE.HELM;

            case LAYERMODE.HELM:
                layerMask |= (1 << 8); // Helm
                goto case LAYERMODE.DECK;

            case LAYERMODE.DECK:
                layerMask |= (1 << 9); // Deck
                goto case LAYERMODE.CABIN;

            case LAYERMODE.CABIN:
                layerMask |= (1 << 10); // Cabin
                break;

            default:
                break;
        }

        // Painfully render appropiate objects to be invisible
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in objects)
        {
            var tileScript = tile.GetComponent<TileScript>();

            if ((LAYERMODE)tileScript.myLayer >= lm)
            {
                tile.GetComponent<Renderer>().enabled = true;
                tile.layer = 6; // Grid layer
            }
            else
            {
                tile.GetComponent<Renderer>().enabled = false;
                tile.layer = 11; // Invisible layer
            }
        }

        // Apply culling
        this.GetComponent<Camera>().cullingMask = layerMask;
    }
}
