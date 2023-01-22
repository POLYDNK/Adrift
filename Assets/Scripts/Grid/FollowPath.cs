using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    // Movespeed
    [SerializeField] public float moveSpeed = 3.0f;

    // A stack of path nodes that link to the position of tiles
    public Stack<PathTreeNode> pathToFollow = new Stack<PathTreeNode>();
    // Raw distance for ability movement
    public Vector2Int abilityDist;

    // Current target
    private GameObject targetTile;
    private Vector3 targetPos;
    private Vector3 targetDirection;

    // Distances
    private float distToTarget;
    private float moveDist;

    // Flags
    private bool tileSet = false;
    private bool wasAbility;

    // Audio
    AudioSource audioData;

    // Initialization
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play();
        audioData.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        // Ability movement
        if(abilityDist.x != 0 || abilityDist.y != 0) {
            if(abilityDist.x > 0) abilityDist.x--;
            else if(abilityDist.x < 0) abilityDist.x++;
            if(abilityDist.y > 0) abilityDist.y--;
            else if(abilityDist.y < 0) abilityDist.y++;
            var character = this.gameObject.GetComponent<CharacterStats>();
            targetTile = character.myGrid.GetComponent<GridBehavior>().GetTileAtPos(character.gridPosition + new Vector2Int(abilityDist.x != 0 ? (int) Mathf.Sign(abilityDist.x) : 0, abilityDist.y != 0 ? (int) Mathf.Sign(abilityDist.y) : 0));
            float targetDist = Vector3.Distance(transform.position, targetPos);
            if(targetTile != null && targetDist > 0.0f) {
                tileSet = true;
                wasAbility = true;
            }
        }
        else {

        // Audio
        if (pathToFollow.Count > 0)
        {
            if (audioData.isPlaying == false)
            {
                audioData.UnPause();
            }
        }
        else
        {
            audioData.Pause();
        }

        // Get a new tile to travel to when these conditions are met
        while (pathToFollow != null && pathToFollow.Count > 0 && !tileSet)
        {
            // Get the next node
            targetTile = pathToFollow.Pop().myTile;

            // Test whether the target position is different from the current position
            distToTarget = Vector3.Distance(transform.position, targetPos);

            //Debug.Log("distToTarget: " + distToTarget);

            if (distToTarget > 0.0f)
            {
                // Debug msg
                //Debug.Log("Follow Path: moving " + this.name + " from " + transform.position.ToString() + " to " + targetPos.ToString());

                // Target tile is set
                tileSet = true;
                wasAbility = false;
            }
        }
        }

        // When we have a tile to travel to
        if (tileSet)
        {
            this.gameObject.GetComponent<Animator>().SetBool("isMoving", true);
            // Get the position of the corresponding tile
            Vector3 pos = targetTile.transform.position;

            // Set the target position to the position of the tile
            targetPos = new Vector3(pos.x, pos.y + 0.5f, pos.z);
            targetDirection = (targetPos - transform.position).normalized;
            distToTarget = Vector3.Distance(transform.position, targetPos);
            moveDist = Vector3.Distance(new Vector3(0,0,0), targetDirection * (wasAbility ? 8.0f : moveSpeed) * Time.deltaTime);

            // Check whether we reached the target
            if (distToTarget > 0.0f && distToTarget <= moveDist)
            {
                transform.position = targetPos;
                tileSet = false;
            }
            // Else, move this object towards it
            else
            {
                transform.position += targetDirection * (wasAbility ? 8.0f : moveSpeed) * Time.deltaTime;

                // Update y-axis rotation
                if(!wasAbility) this.gameObject.GetComponent<CharacterStats>().rotateTowards(targetPos);
            }
        }
        else this.gameObject.GetComponent<Animator>().SetBool("isMoving", false);
    }

    public bool isMoving() {
        return tileSet;
    }
}
