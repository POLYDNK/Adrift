using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//DEPRECATED
public class CannonUI : MonoBehaviour
{
    [SerializeField] public Button fireCannonball;
    [SerializeField] public Button fireCharacter;
    [SerializeField] public Image fireCannonballImage;
    [SerializeField] public Image fireCharacterImage;
    [SerializeField] public float battleSleepTime;

    private GameObject currCannon = null;
    private GameObject grid;
    private Vector2Int activePos = new Vector2Int(-1,-1);

    // Camera Ref
    [SerializeField] public CameraController camScript;

    // Update is called once per frame
    void Update()
    {
        GetAdjacentCannon();

        if (currCannon == null)
        {
            fireCannonball.interactable = false;
            fireCharacter.interactable = false;
        }
        else
        {
            fireCannonball.interactable = true;
            fireCharacter.interactable = true;
        }
    }

    void LateUpdate()
    {
        // Show UI above current cannon
        if (currCannon != null)
        {
            fireCannonball.enabled = true;
            fireCharacter.enabled = true;
            fireCannonballImage.enabled = true;
            fireCharacterImage.enabled = true;

            // Offsets
            Vector3 cannonOffset = new Vector3(-0.8f, 3.0f, 0);
            Vector3 characterOffset = new Vector3(0.8f, 3.0f, 0);

            // Update position
            fireCannonball.transform.position = Camera.main.WorldToScreenPoint(currCannon.transform.position + cannonOffset);
            fireCharacter.transform.position = Camera.main.WorldToScreenPoint(currCannon.transform.position + characterOffset);

            // Update Scale
            float camDist = Vector3.Distance(Camera.main.transform.position, currCannon.transform.position);
            float newScale = 0.0f;

            if (camDist != 0.0f)
            {
                newScale = Mathf.Max(9.0f / camDist, 0.1f);
            }

            fireCannonball.transform.localScale = new Vector3(newScale, newScale, newScale);
            fireCharacter.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        else
        {
            fireCannonball.enabled = false;
            fireCharacter.enabled = false;
            fireCannonballImage.enabled = false;
            fireCharacterImage.enabled = false;
        }
    }

    public void FireCannon()
    {
        if (BattleEngine.Instance.active)
        {
            // Fire Cannon
            //currCannon.transform.GetChild(0).GetComponent<CannonObject>().fireCannonball();

            // Sleep Battle Engine for a Moment
            BattleEngine.Instance.StartCoroutine(BattleEngine.Instance.PauseBattleEngine(battleSleepTime, true));
        }
    }

    public void FireSelf()
    {
        if (BattleEngine.Instance.active)
        {
            // Fire Effect
            var cannonScript = currCannon.transform.GetChild(0).GetComponent<CannonObject>();

            // Fire Active Character
            Transform modelTrans = BattleEngine.Instance.activeUnit.transform.GetChild(0);
            var modelScript = modelTrans.GetComponent<CharacterCollision>();
            modelScript.WakeRigidBody();
            cannonScript.FireObject(modelTrans.gameObject);

            // Set Camera Follow
            camScript.SetCameraFollow(modelTrans.gameObject);

            // Sleep Battle Engine for a Moment
            BattleEngine.Instance.StartCoroutine(BattleEngine.Instance.PauseBattleEngine(battleSleepTime, true));
        }
    }

    private void GetAdjacentCannon()
    {
        // Only get if the active character grid pos has changed
        if (activePos != BattleEngine.Instance.activeUnitPos && BattleEngine.Instance.init == true)
        {
            Debug.Log("Searching for cannon at " + activePos.ToString());

            activePos = BattleEngine.Instance.activeUnitPos;
            grid = BattleEngine.Instance.activeGrid.gameObject;
            var gridScript = grid.GetComponent<Grid>();
        
            if (CheckIfCannon(gridScript.GetTileNorth(activePos)))
            {
                currCannon = gridScript.GetTileNorth(activePos).GetComponent<Tile>().objectOn;
            }
            else if (CheckIfCannon(gridScript.GetTileEast(activePos)))
            {
                currCannon = gridScript.GetTileEast(activePos).GetComponent<Tile>().objectOn;
            }
            else if (CheckIfCannon(gridScript.GetTileSouth(activePos)))
            {
                currCannon = gridScript.GetTileSouth(activePos).GetComponent<Tile>().objectOn;
            }
            else if (CheckIfCannon(gridScript.GetTileWest(activePos)))
            {
                currCannon = gridScript.GetTileWest(activePos).GetComponent<Tile>().objectOn;
            }
            else
            {
                currCannon = null;
            }
        }
    }

    private bool CheckIfCannon(GameObject tile)
    {
        bool isCannon = false;

        if (tile != null)
        {
            var tileScript = tile.GetComponent<Tile>();
            if (tileScript.objectOn != null && tileScript.objectOn.tag == "Cannon")
            {
                isCannon = true;
            }
        }

        return isCannon;
    }
}
