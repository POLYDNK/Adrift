using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

//Main controller for the battle system
//Note that this system must be activated and will not perform any logic until it is
public class BattleEngine : MonoBehaviour 
 {
    //Morale vars
    public const string MoraleBuffID = "morale_buff";
    public const string MoraleDebuffID = "morale_debuff";
    public const int MoraleHigh = 75;
    public const int MoraleLow = 25;

    public static readonly StatModifier[] MoraleBuffs = {
        new(StatType.Str, OpType.Multiply, -1, 0.2f, 1f, MoraleBuffID),
        new(StatType.Def, OpType.Multiply, -1, 0.2f, 1f, MoraleBuffID),
        new(StatType.Spd, OpType.Multiply, -1, 0.2f, 1f, MoraleBuffID),
        new(StatType.Dex, OpType.Multiply, -1, 0.2f, 1f, MoraleBuffID)
    };
    public static readonly StatModifier[] MoraleDebuffs = {
        new(StatType.Str, OpType.Multiply, -1, -0.2f, 1f, MoraleDebuffID),
        new(StatType.Def, OpType.Multiply, -1, -0.2f, 1f, MoraleDebuffID),
        new(StatType.Spd, OpType.Multiply, -1, -0.2f, 1f, MoraleDebuffID),
        new(StatType.Dex, OpType.Multiply, -1, -0.2f, 1f, MoraleDebuffID)
    };
    //Prefabs
    public GameObject buttonPrefab;

    public List<GameObject> grids = new(); //All grids
    public GameObject playerCrew, enemyCrew, playerShip, enemyShip; //Crews & ships
    public List<GameObject> units = new(); //All units
    public PathTreeNode GridPaths;
    public bool active = false; //Activation flag to be set by other systems
    public bool interactable = true; //Flag used for locking actions during events
    public bool surrendered = false, victory = false, defeat = false; //End of battle flags

    public bool moving = false; //Whether the current mode is moving or acting
    private Ability selectedAbility;
    public bool init = false;
    public bool isPlayerTurn;
    private bool moved = false; //Whether movement was taken
    private bool acted = false; //Whether an action was taken
    private int turnCount = 0;
    public GameObject activeUnit, activeUnitTile;
    public GameObject grid; //Active grid that unit is on
    public Vector2Int activeUnitPos;
    public List<GameObject> aliveUnits = new();
    public List<GameObject> deadUnits = new();
    public List<GameObject> turnQueue = new(); //Stored units in the turn queue (units can repeat)

    //UI references
    [SerializeField] public CharacterCard charCard;
    [FormerlySerializedAs("crewTurnOrder")] [SerializeField] public BattleTurnManager crewTurnManager;
    public GameObject canvas, actionCoin, moveCoin, endCoin, surrenderCoin;
    private GameObject lastCoin;
    private Animator actionCoinAnimator, moveCoinAnimator, endCoinAnimator, surrenderCoinAnimator;
    private List<Button> actionButtons = new();
    private List<Ability> usedAbilities = new(); //Abilities used this turn
    private GameObject victoryText, defeatText, surrenderText;
    private RectangleBar playerShipBar, playerMoraleBar, enemyShipBar, enemyMoraleBar;

    //Click Detection
    private Camera cam, canvasCam;
    private Renderer rend;
    private RaycastHit hit;
    private static readonly int GridMask = 1 << 6, CoinMask = 1 << 12;

    //Selection Management
    private bool charSelected = false;
    private bool charHighlighted = false;
    private Vector2Int selectedCharPos;
    private Vector2Int highlightedCharPos;
    private GameObject selectedGrid;
    private int lastXDist, lastYDist; //Last distance between user and target for ability selection

    //AI Variables (OLD)
    private PlayerActionList playerActions = new();
    private GameObject playerTarget; //Might be unnecessary
    private static readonly int Hovered = Animator.StringToHash("hovered");
    private static readonly int Selected = Animator.StringToHash("selected");
    private static readonly int Interactable = Animator.StringToHash("interactable");

    // AI Variables (NEW)
    private EnemyAIController AIController;

    // Start is called before the first frame update
    void Start() 
    {
        // Setup AI Controller
        AIController = new EnemyAIController(this);

        // Temporary assignment of ships, crews should be passed in somewhere since they're permanent
        playerCrew.GetComponent<Crew>().ship = playerShip;
        enemyCrew.GetComponent<Crew>().ship = enemyShip;

        // Get Camera
        cam = Camera.main;
        canvasCam = GameObject.Find("Canvas Camera").GetComponent<Camera>();

        // Set coin animators
        actionCoinAnimator = actionCoin.GetComponent<Animator>();
        moveCoinAnimator = moveCoin.GetComponent<Animator>();
        endCoinAnimator = endCoin.GetComponent<Animator>();
        surrenderCoinAnimator = surrenderCoin.GetComponent<Animator>();

        // Get and Set Victory and Defeat Text
        victoryText = GameObject.Find("VictoryText");
        victoryText.SetActive(false);
        defeatText = GameObject.Find("DefeatText");
        defeatText.SetActive(false);
        surrenderText = GameObject.Find("SurrenderText");
        surrenderText.SetActive(false);

        // Health & Morale bars
        playerShipBar = GameObject.Find("PlayerShipBar").GetComponent<RectangleBar>();
        int playerShipHealth = playerCrew.GetComponent<Crew>().GetShip().hp;
        playerShipBar.SetMaxHealth(playerShipHealth);
        playerShipBar.SetHealth(playerShipHealth);

        playerMoraleBar = GameObject.Find("PlayerMoraleBar").GetComponent<RectangleBar>();
        int playerMorale = playerCrew.GetComponent<Crew>().morale;
        playerMoraleBar.SetMaxHealth(100);
        playerMoraleBar.SetHealth(playerMorale);

        enemyShipBar = GameObject.Find("EnemyShipBar").GetComponent<RectangleBar>();
        int enemyShipHealth = enemyCrew.GetComponent<Crew>().GetShip().hp;
        enemyShipBar.SetMaxHealth(enemyShipHealth);
        enemyShipBar.SetHealth(enemyShipHealth);

        enemyMoraleBar = GameObject.Find("EnemyMoraleBar").GetComponent<RectangleBar>();
        int enemyMorale = enemyCrew.GetComponent<Crew>().morale;
        enemyMoraleBar.SetMaxHealth(100);
        enemyMoraleBar.SetHealth(enemyMorale);
    }

    // Update is called once per frame
    void Update()
    {
        // Keyboard control
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SelectAction(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SelectMove(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { EndTurn(); }
        
        // Check whether the battle system is active
        if(active)
        {            
            if(!init)
            {
                InitializeBattleSystem();
            }
            else 
            {
                var gridTiles = grid.GetComponent<Grid>();
                bool mouseOnGrid = false;
                // Handle 3D UI interactions
                if (Physics.Raycast(canvasCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, CoinMask))
                {
                    GameObject coin = hit.transform.gameObject;
                    if(lastCoin != coin)
                    {
                        if(lastCoin != null)
                        {
                            Animator lastCoinAnimator = lastCoin.GetComponent<Animator>();
                            lastCoinAnimator.SetBool(Hovered, false);
                            if(!lastCoinAnimator.GetBool(Selected)) lastCoin.transform.GetChild(0).gameObject.SetActive(false); //Disable text of old coin
                        }
                        coin.GetComponent<Animator>().SetBool(Hovered, true);
                        hit.transform.GetChild(0).gameObject.SetActive(true); //Enable text of new coin
                    }
                    else coin.GetComponent<Animator>().SetBool(Hovered, false);

                    if(Input.GetMouseButtonDown(0))
                    {
                        Animator coinAnimator = coin.GetComponent<Animator>();
                        // Select coin if it's interactable
                        if(coinAnimator.GetBool(Interactable))
                        {
                            SelectCoin(actionCoin, actionCoinAnimator, false);
                            SelectCoin(moveCoin, moveCoinAnimator, false);
                            SelectCoin(endCoin, endCoinAnimator, false);
                            SelectCoin(surrenderCoin, surrenderCoinAnimator, false);
                            SelectCoin(coin, coinAnimator, true);
                        }
                        // Handle various click actions
                        if(coin == actionCoin && actionCoinAnimator.GetBool(Interactable)) SelectAction();
                        else if(coin == moveCoin && moveCoinAnimator.GetBool(Interactable)) SelectMove();
                        else if(coin == endCoin && endCoinAnimator.GetBool(Interactable)) EndTurn();
                        else if(coin == surrenderCoin && surrenderCoinAnimator.GetBool(Interactable)) Surrender();
                    }
                    lastCoin = coin;
                }
                // Handle grid interactions
                else
                {
                    if(lastCoin != null)
                    {
                        Animator lastCoinAnimator = lastCoin.GetComponent<Animator>();
                        lastCoinAnimator.SetBool(Hovered, false);
                        if(!lastCoinAnimator.GetBool(Selected)) lastCoin.transform.GetChild(0).gameObject.SetActive(false); //Disable text of old coin
                        lastCoin = null;
                    }
                    
                    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, GridMask))
                    {
                        mouseOnGrid = true;
                        // Get data from tile hit
                        var objectHit   = hit.transform;
                        var selectRend  = objectHit.GetComponent<Renderer>();
                        var tileScript  = objectHit.GetComponent<TileScript>();
                        Vector2Int tilePos = tileScript.position;
                        int xDist = activeUnitPos.x - tilePos.x;
                        int yDist = activeUnitPos.y - tilePos.y;

                        // ----------------------------
                        // Character Movement & Actions
                        // ----------------------------
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Movement
                            if(moving) 
                            {
                                // If a tile has a character on it, then we can only select it
                                if (tileScript.hasCharacter)
                                {
                                    SelectCharacter(objectHit.gameObject);
                                }
                                // If we have a selected character/tile, then we can move it to any
                                // highlighted tile without a character already on it
                                else
                                {
                                    Debug.Log("Moving " + activeUnit);
                                    Debug.Log(gridTiles.GetCharacterAtPos(selectedCharPos));
                                    MoveUnit(tilePos, false);
                                }
                            }
                            // Action
                            else ActUnit(tilePos, false);
                        }
                        else
                        // -------------------------------
                        // Character and Tile Highlighting
                        // -------------------------------
                        {
                            if(charSelected == false && moving) //Moving
                            {
                                bool okayToHighlight = true;

                                // Prevent highlighting the tile w/ a selected
                                // character on it
                                if (charSelected && selectedCharPos == tilePos)
                                {
                                    okayToHighlight = false;
                                }

                                // Cannot highlight tiles without a character on it
                                if (tileScript.hasCharacter == false)
                                {
                                    okayToHighlight = false;
                                }

                                // If the mouse moved away from the highlighted tile,
                                // then unhighlight it
                                if (charHighlighted && highlightedCharPos != tilePos)
                                {
                                    gridTiles.Tiles[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = IsTileActive(highlightedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
                                }

                                // Highlight mouse over tile if it's okay to highlight it
                                if (okayToHighlight)
                                {
                                    selectRend.material = IsTileActive(highlightedCharPos) ? gridTiles.activeHighlighted : gridTiles.highlighted;
                                    highlightedCharPos = tilePos;
                                    charHighlighted = true;
                                }
                            }
                            else if(!moving && !acted) { //Acting
                                if(charHighlighted && highlightedCharPos != tilePos) {
                                    foreach(GameObject selTile in gridTiles.GetAllTiles()) {
                                        var selPos = selTile.GetComponent<TileScript>().position;
                                        if(selPos != activeUnitPos && selTile.GetComponent<TileScript>().passable) selTile.GetComponent<Renderer>().material = IsTileActive(selPos) ? gridTiles.activeUnselected : (Mathf.Abs(activeUnitPos.x - selPos.x) + Mathf.Abs(activeUnitPos.y - selPos.y) > selectedAbility.range ? gridTiles.unselected : gridTiles.abilityHighlighted);
                                    }
                                }
                                if(selectedAbility != null && tileScript.highlighted && Mathf.Abs(activeUnitPos.x - tilePos.x) + Mathf.Abs(activeUnitPos.y - tilePos.y) <= selectedAbility.range) {
                                    foreach(Vector2Int pos in selectedAbility.GetRelativeShape(xDist, yDist)) {
                                        var selPos = new Vector2Int(tilePos.x + pos.x, tilePos.y + pos.y);
                                        var selTile = gridTiles.GetTileAtPos(selPos);
                                        if(selTile != null) {
                                            var selTileScript = selTile.GetComponent<TileScript>();
                                            if(selTileScript.passable && selTileScript.characterOn != activeUnit) {
                                                if(pos.x == 0 && pos.y == 0) {
                                                    highlightedCharPos = selPos;
                                                    charHighlighted = true;
                                                }
                                                selTileScript.highlighted = true;
                                                selTile.GetComponent<Renderer>().material = gridTiles.ability;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        lastXDist = xDist;
                        lastYDist = yDist;
                    }
                }
                if(!mouseOnGrid) {
                    if (charHighlighted)
                    {
                        if(moving) gridTiles.Tiles[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = IsTileActive(highlightedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
                        /*else if(selectedAbility != null) {
                            foreach(Vector2Int pos in selectedAbility.getRelativeShape(lastXDist, lastYDist)) {
                                var selPos = new Vector2Int(highlightedCharPos.x + pos.x, highlightedCharPos.y + pos.y);
                                var selTile = gridTiles.GetTileAtPos(selPos);
                                if(selTile != null && selTile.GetComponent<TileScript>().passable) gridTiles.grid[selPos.x, selPos.y].GetComponent<Renderer>().material = isTileActive(selPos) ? gridTiles.activeUnselected : (Mathf.Abs(activeUnitPos.x - selPos.x) + Mathf.Abs(activeUnitPos.y - selPos.y) > selectedAbility.range ? gridTiles.unselected : gridTiles.abilityHighlighted);
                            }
                        }*/
                        charHighlighted = false;
                    }
                }
            }
        }
    }

    private void InitializeBattleSystem()
    {
        Debug.Log("BattleEngine initializing...");
        // ------------------------------------

        // Add all characters to be controlled by the battle system
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject character in characters)
        {
            units.Add(character);
            aliveUnits.Add(character);
        }

        // Apply morale modifiers
        //TODO: Pass in crews properly, this is just editing prefabs right now because it's not instantiated properly
        /*CrewSystem playerCrew = getPlayerCrew(), enemyCrew = getEnemyCrew();
        if(playerCrew.morale >= MORALE_HIGH) foreach(StatModifier modifier in MORALE_BUFFS) playerCrew.addModifier(modifier);
        else if(playerCrew.morale <= MORALE_LOW) foreach(StatModifier modifier in MORALE_DEBUFFS) playerCrew.addModifier(modifier);
        if(enemyCrew.morale >= MORALE_HIGH) foreach(StatModifier modifier in MORALE_BUFFS) enemyCrew.addModifier(modifier);
        else if(enemyCrew.morale <= MORALE_LOW) foreach(StatModifier modifier in MORALE_DEBUFFS) enemyCrew.addModifier(modifier);*/

        //Set up turns
        PickNewTurn();

        init = true;

        // ------------------------------------
        Debug.Log("BattleEngine initialized.");
    }

    public void ResetAllHighlights()
    {
        GameObject currentTile;
        var gridTiles = grid.GetComponent<Grid>();
        for (int x = 0; x < gridTiles.width; x++)
        {
            for (int y = 0; y < gridTiles.height; y++)
            {
                // Get data from tile at current pos
                currentTile = gridTiles.GetTileAtPos(new Vector2Int(x,y));
                var currentTileScript = currentTile.GetComponent<TileScript>();

                // Check if the tile is highlighted
                if (currentTileScript.highlighted)
                {
                    // Reset highlighting
                    currentTile.GetComponent<Renderer>().material = IsTileActive(new Vector2Int(x,y)) ? gridTiles.activeUnselected : gridTiles.unselected;
                    currentTileScript.highlighted = false;
                }
            }
        }
    }

    public void SelectCharacter(GameObject tile) {
        TileScript tileScript = tile.GetComponent<TileScript>();
        var tilePos = tileScript.position;
        // If a tile has a character on it, then we can only select it
        if (tileScript.hasCharacter)
        {
            var gridTiles = grid.GetComponent<Grid>();
            // Move Camera to clicked character
            cam.GetComponent<CameraController>().LookAtPos(tile.transform.position);

            // Case 1: if there's no selected character, then just select this one
            if (charSelected == false) 
            {
                if(moving) SetupMove(tile);
                else SetupAction(tile);
            }
            // Case 2: if the character is the same as the already selected,
            // then unselect it
            else if (tilePos == selectedCharPos)
            {
                ResetAllHighlights();
                tile.transform.GetComponent<Renderer>().material = IsTileActive(tilePos) ? gridTiles.activeUnselected : gridTiles.unselected;
                Debug.Log("Character on " + tile.name + " has been unselected");
                charSelected = false;
                charCard.Close();
            }
            // Case 3: if the character is different than the currently selected character,
            // then deselect the current character and select the new one
            else 
            {
                if(moving) SetupMove(tile);
                else SetupAction(tile);
            }
        }
    }

    // Highlights available action tiles for abilities
    public void HighlightActionTiles(Vector2Int pos, int range) {
        var gridTiles = grid.GetComponent<Grid>();
        ResetAllHighlights();
        for(int x = Mathf.Max(pos.x - range, 0); x <= pos.x + range; x++) {
            for(int y = Mathf.Max(pos.y - range, 0); y <= pos.y + range; y++) {
                if(Mathf.Abs(x - pos.x) + Mathf.Abs(y - pos.y) <= range) {
                    var tilePos = new Vector2Int(x, y);
                    var tile = gridTiles.GetTileAtPos(tilePos);
                    if(tile == null) continue;
                    var tileScript = tile.GetComponent<TileScript>();
                    if(tileScript.passable) {
                        tileScript.highlighted = true;
                        tile.GetComponent<Renderer>().material = IsTileActive(tilePos) ? gridTiles.activeHighlighted : gridTiles.abilityHighlighted;
                    }
                }
            }
        }
    }

    // Highlights available move tiles from a provided position and range
    public void HighlightValidMoves(Vector2Int pos, int range)
    {
        // Get script from grid
        var gridScript = grid.GetComponent<Grid>();

        // Only hightlight if the character as pos is the active unit
        if(gridScript.GetCharacterAtPos(pos) == activeUnit)
        {
            // Highlight move tiles
            ResetAllHighlights();
            GridPaths = gridScript.GetAllPathsFromTile(gridScript.GetTileAtPos(pos), range);
            HighlightPathTree(GridPaths);
        }
    }

    public void HighlightPathTree(PathTreeNode root) {
        // Get data from tile
        var gridTiles = grid.GetComponent<Grid>();
        var tileRend = root.MyTile.GetComponent<Renderer>();
        var tileScript = root.MyTile.GetComponent<TileScript>();
        Vector2Int tilePos = tileScript.position;

        // Highlight tile
        tileRend.material = IsTileActive(tilePos) ? gridTiles.activeHighlighted : gridTiles.highlighted;
        if(root.Up != null) HighlightPathTree(root.Up);
        if(root.Down != null) HighlightPathTree(root.Down);
        if(root.Left != null) HighlightPathTree(root.Left);
        if(root.Right != null) HighlightPathTree(root.Right);
    }

    public bool IsTileActive(Vector2Int tilePos) {
        var gridTiles = grid.GetComponent<Grid>();
        return gridTiles.Tiles[tilePos.x, tilePos.y].GetComponent<TileScript>().characterOn == activeUnit;
    }

    public void SelectAction() {
        if (interactable)
        {
            moving = false;
            SelectCoin(actionCoin, actionCoinAnimator, true);
            ShowActionsList();
            actionButtons[0].onClick.Invoke();
            SetupAction(activeUnitTile);
            actionButtons[0].Select(); //Temp highlight for basic attack button
        }
    }

    public void SelectMove() 
    {
        if (interactable)
        {
            HideActionsList();
            moving = true;
            SelectCoin(moveCoin, moveCoinAnimator, true);
            SetupMove(activeUnitTile);
        }
    }

    public void PickNewTurn() 
    {
        if(active)
        {
            StartTurn();
        }
    }

    public void UpdateTurnOrder() {
        List<Vector2Int> temp = new List<Vector2Int>();
        //Populate list with sorted indices of units by speed over the next 50 turns
        for(int i = 1; i < 50 + turnCount; i++) {
            for(int j = 0; j < units.Count; j++) {
                Character unit = units[j].GetComponent<Character>();
                if(!unit.IsDead()) temp.Add(new Vector2Int(j, (51 - unit.GetSpeed()) * i));
            }
        }
        temp = temp.OrderBy(v => v.y).ToList(); //OrderBy is stable, Sort is not
        //Fill turn queue with the actual units
        turnQueue.Clear();
        foreach(Vector2Int vec in temp) {
            turnQueue.Add(units[vec.x]);
            if(turnQueue.Count == 50 + turnCount - 1) break;
        }
        for(int i = 0; i < turnCount - 1; i++) turnQueue.RemoveAt(0); //Cull turns that were already taken

        // Update crew turn order in the UI
        crewTurnManager.UpdateCrewTurnOrder();
    }

    //Start a new turn for the active unit
    public void StartTurn()
    {
        // Increment turn count and populate turn queue if empty
        turnCount++;
        UpdateTurnOrder();

        // Get active unit script
        activeUnit = turnQueue[0];
        var activeUnitScript = activeUnit.GetComponent<Character>();

        // Recover 1 AP
        activeUnitScript.AddAP(1);

        // Get data from active unit
        grid = activeUnitScript.myGrid;
        var gridScript = grid.GetComponent<Grid>();
        activeUnitPos = activeUnitScript.gridPosition;

        // Camera Culling
        var camScript = cam.GetComponent<CameraController>();
        camScript.SetLayerMode((CameraController.Layermode)activeUnit.layer);

        // Deselect ability
        selectedAbility = null;
        usedAbilities.Clear();
        
        // Set the tile of which the character is on to be active
        activeUnitTile = gridScript.Tiles[activeUnitPos.x, activeUnitPos.y];
        activeUnitTile.GetComponent<Renderer>().material = gridScript.activeUnselected;
        camScript.LookAtPos(activeUnitTile.transform.position);

        // Setup coins based on whether it's the player's turn
        isPlayerTurn = IsAllyUnit(activeUnit);
        actionCoinAnimator.SetBool(Interactable, isPlayerTurn);
        moveCoinAnimator.SetBool(Interactable, isPlayerTurn);
        endCoinAnimator.SetBool(Interactable, isPlayerTurn);
        surrenderCoinAnimator.SetBool(Interactable, isPlayerTurn);

        if(!isPlayerTurn)
        {
            SelectCoin(actionCoin, actionCoinAnimator, false);
            SelectCoin(moveCoin, moveCoinAnimator, false);
            SelectCoin(endCoin, endCoinAnimator, false);
            SelectCoin(surrenderCoin, surrenderCoinAnimator, false);
            DoAITurn();
        }
        else 
        {
            SelectCoin(actionCoin, actionCoinAnimator, false);
            SelectCoin(moveCoin, moveCoinAnimator, true);
            SelectCoin(endCoin, endCoinAnimator, false);
            SelectCoin(surrenderCoin, surrenderCoinAnimator, false);
            RefreshActionButtons();
            SelectMove(); //Default to move (generally units move before acting)
        }

        // Heatmap Generation
        HeatmapGenerator.GenerateHeatmap(aliveUnits, activeUnit);
    }

    //End the active unit's turn
    public void EndTurn() 
    {
        if (interactable)
        {
            var gridTiles = grid.GetComponent<Grid>();
            gridTiles.GetTileAtPos(activeUnitPos).GetComponent<TileScript>().highlighted = false;
            gridTiles.GetTileAtPos(activeUnitPos).GetComponent<Renderer>().material = gridTiles.unselected;
            
            // Tick stat modifiers
            activeUnit.GetComponent<Character>().TickModifiers();

            //Player turn enqueue handling - passes in the current active unit (if it is a player controlled unit), 
            //the target of this turn's action (if any), the type of action taken this turn (if any), and whether the character moved
            if(isPlayerTurn && selectedAbility != null && acted && playerTarget != null)
            {
                //Add the new PlayerAction to the playerActions queue, using the overloaded constructor
                playerActions.Add(new PlayerAction(activeUnit.GetComponent<Character>(), playerTarget.GetComponent<Character>(), selectedAbility, moved));

                //Logging to show what was just enqueued
                Debug.Log("AI: playerActions Enqueue: " + playerActions[playerActions.Count() - 1].GetCharacter().displayName + " " 
                + playerActions[playerActions.Count() - 1].GetTarget().displayName + " "
                + playerActions[playerActions.Count() - 1].GetAbility().displayName + " " 
                + playerActions[playerActions.Count() - 1].GetMovement() 
                + "\n" + "Queue Size: " + playerActions.Count());
            }

            //If the player didn't act, don't send in a target
            else if(isPlayerTurn && acted == false)
            {
                playerActions.Add(new PlayerAction(activeUnit.GetComponent<Character>(), selectedAbility, moved));

                //Logging to show what was just enqueued
                Debug.Log("AI: playerActions Enqueue: " + playerActions[playerActions.Count() - 1].GetCharacter().displayName + " "
                + playerActions[playerActions.Count() - 1].GetMovement() 
                + "\n" + "Queue Size: " + playerActions.Count());
            }

            turnQueue.RemoveAt(0);
            moved = false;
            acted = false;
            LogicUpdate();
            //Move ships
            StartCoroutine(EndTurnMoveShips());
        }
    }
    
    public IEnumerator EndTurnMoveShips()
    {
        // Before wait
        active = false;
        interactable = false;

        DisableCoins();
        
        cam.GetComponent<CameraController>().SetCameraFollow(activeUnit);
        playerShip.GetComponent<ShipController>().moveSpeed = 1.5F;
        yield return new WaitForSecondsRealtime(0F);
        playerShip.GetComponent<ShipController>().moveSpeed = 0F;
        EnableCoins();

        // After wait
        active = true;
        interactable = true;
        LogicUpdate();
        PickNewTurn();
    }

    public void SetupMove(GameObject objectTile) 
    {
        SetupActionOrMove(objectTile, false);
    }

    public void SetupAction(GameObject objectTile)
    {
        SetupActionOrMove(objectTile, true);
    }

    private void SetupActionOrMove(GameObject tile, bool isAction)
    {
        var gridTiles = grid.GetComponent<Grid>();
        var selectRend  = tile.GetComponent<Renderer>();
        var tileScript  = tile.GetComponent<TileScript>();
        Vector2Int tilePos = tileScript.position;

        // Unselect currently selected character
        if(charSelected) {
            var selectedGrid = this.selectedGrid.GetComponent<Grid>();
            Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
            selectedGrid.Tiles[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = IsTileActive(selectedCharPos) ? selectedGrid.activeUnselected : selectedGrid.unselected;
        }

        ResetAllHighlights();

        if (isAction)
        {
            if(!acted) { HighlightActionTiles(tilePos, selectedAbility.range); }
        }
        else
        {
            if(!moved)
            {
                cam.GetComponent<CameraController>().SetCameraFollow(tileScript.characterOn);
                HighlightValidMoves(tilePos, tileScript.characterOn.GetComponent<Character>().GetMovement());
            }
        }

        // Select new tile at tile pos
        selectRend.material = IsTileActive(tilePos) ? gridTiles.activeSelected : gridTiles.selected;
        selectedCharPos = tilePos;
        selectedGrid = grid;

        Debug.Log("Character on " + tile.name + " has been selected");

        // Selection Logic
        charSelected = true;
        charCard.Open(tileScript.characterOn.GetComponent<Character>());
        charHighlighted = false;
    }

    //Try to use the selected ability at the specified position on the grid. Returns true if action succeeds. Will not affect game state if simulate is true.
    public bool ActUnit(Vector2Int tilePos, bool simulate) 
    {
        Debug.Log("Moving: " + moving.ToString()
        + " || Acted: " + acted.ToString()
        + " || Selected Ability: " + selectedAbility.displayName 
        + " || activeUnit AP: " + activeUnit.GetComponent<Character>().ap
        + " || Selected Ability Cost: " + selectedAbility.costAP);

        if(moving || acted || selectedAbility == null || activeUnit.GetComponent<Character>().ap < selectedAbility.costAP)
        {
            return false;
        }
        
        var gridTiles = grid.GetComponent<Grid>();
        var tileScript = gridTiles.GetTileAtPos(tilePos).GetComponent<TileScript>();
        int dist = Mathf.Abs(activeUnitPos.x - tilePos.x) + Mathf.Abs(activeUnitPos.y - tilePos.y); //Take Manhattan distance
        if(dist > selectedAbility.range) return false;
        Debug.Log("Past Manhattan distance");
        GameObject selectedCharacter = null;
        if(selectedAbility.requiresTarget) 
        { 
            Debug.Log("Selected Target Check");
            //Check for valid target
            if(!tileScript.hasCharacter) return false;
            if(selectedAbility.friendly && !AllianceChecker(gridTiles.GetCharacterAtPos(tilePos))) return false;
            if(!selectedAbility.friendly && AllianceChecker(gridTiles.GetCharacterAtPos(tilePos))) return false;
            selectedCharacter = tileScript.characterOn;
        }
        if(simulate) return true;

        int xDist = activeUnitPos.x - tilePos.x;
        int yDist = activeUnitPos.y - tilePos.y;
        List<GameObject> characters = new List<GameObject>();
        //Select characters based on ability shape
        foreach(Vector2Int pos in selectedAbility.GetRelativeShape(xDist, yDist))
        {
            Vector2Int selPos = new Vector2Int(tilePos.x + pos.x, tilePos.y + pos.y);
            var selTile = gridTiles.GetTileAtPos(selPos);
            if(selTile == null) continue;
            var selTileScript = selTile.GetComponent<TileScript>();
            if(selTileScript.hasCharacter)
            {
                if(selectedAbility.friendly && !AllianceChecker(gridTiles.GetCharacterAtPos(selPos))) continue;
                if(!selectedAbility.friendly && AllianceChecker(gridTiles.GetCharacterAtPos(selPos))) continue;
                characters.Add(selTileScript.characterOn);
            }
        }

        if(!selectedAbility.free) acted = true;
        activeUnit.GetComponent<Character>().AddAP(-selectedAbility.costAP);
        usedAbilities.Add(selectedAbility);

        StartCoroutine(EndActUnit(selectedCharacter == null ? tilePos : selectedCharacter.GetComponent<Character>().gridPosition, characters, xDist, yDist));
        return true;
    }

    IEnumerator EndActUnit(Vector2Int tilePos, List<GameObject> characters, int xDist, int yDist) {
        DisableCoins();
        foreach(GameObject unit in aliveUnits) if(unit != activeUnit && !characters.Contains(unit)) unit.GetComponent<Character>().HideBars();
        foreach(Button button in actionButtons)
        {
            button.interactable = false;
            button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        if(tilePos != activeUnitPos) yield return new WaitWhile(() => !activeUnit.GetComponent<Character>().RotateTowards(grid.GetComponent<Grid>().GetTileAtPos(tilePos).transform.position)); //Wait for rotation first

        var gridTiles = grid.GetComponent<Grid>();
        //Pull and knockback
        if(selectedAbility.knockback != 0)
        {
            foreach(GameObject character in characters) selectedAbility.ApplyKnockback(character.GetComponent<Character>(), gridTiles, xDist, yDist);
        }
        //Self movement
        Vector2Int newPos = selectedAbility.ApplySelfMovement(activeUnit.GetComponent<Character>(), gridTiles, xDist, yDist);
        if(newPos != activeUnitPos)
        {
            activeUnitPos = newPos;
            activeUnitTile = gridTiles.GetTileAtPos(newPos);
            gridTiles.Tiles[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = IsTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
            selectedCharPos = newPos;
        }
        ResetAllHighlights();
        if(!acted) HighlightActionTiles(newPos, selectedAbility.range);
        for(int i = 0; i < selectedAbility.totalHits; i++)
        {
            selectedAbility.AffectCharacters(activeUnit, characters, i);
            yield return new WaitForSecondsRealtime(0.6f);
        }

        //AI: Forward the target(s) to AI handler for enqueue. Currently only forwards one character - for refactoring later
        if(characters.Count > 0) playerTarget = characters[0];

        // ----- Combo Attacks ------
        if(TryComboAttack(activeUnitPos, tilePos, false)) yield return new WaitForSecondsRealtime(0.6f);
        // --------------------------

        foreach(GameObject unit in aliveUnits) unit.GetComponent<Character>().ShowBars();
        EnableCoins();
        if(!acted) {
            actionButtons[0].onClick.Invoke();
            SetupAction(activeUnitTile);
        }
        if(!moved) {
            if(acted) SelectMove(); //Move to move state if available
        }
        LogicUpdate();
        yield break;
    }

    //Search for a possible combo attack and try it if not simulated. Returns true if a combo occurs.
    public bool TryComboAttack(Vector2Int userPos, Vector2Int selectedPos, bool simulate) {
        var gridTiles = grid.GetComponent<Grid>();
        var tileScript = gridTiles.GetTileAtPos(selectedPos).GetComponent<TileScript>();
        int xDist = userPos.x - selectedPos.x;
        int yDist = userPos.y - selectedPos.y;
        if(selectedAbility.requiresTarget && xDist != yDist && selectedAbility.selfMovement == 0) //No diagonals
        {
            if(Mathf.Abs(xDist) > Mathf.Abs(yDist))
            { //Horizontal cases
                if(xDist > 0) return HandleComboAttack(gridTiles.GetTileWest(selectedPos), tileScript.characterOn, simulate);
                else return HandleComboAttack(gridTiles.GetTileEast(selectedPos), tileScript.characterOn, simulate);
            }
            else
            { //Vertical cases
                if(yDist > 0) return HandleComboAttack(gridTiles.GetTileSouth(selectedPos), tileScript.characterOn, simulate);
                else return HandleComboAttack(gridTiles.GetTileNorth(selectedPos), tileScript.characterOn, simulate);
            }
        }
        return false;
    }

    //Try to perform a combo attack from the specified character to the target
    private bool HandleComboAttack(GameObject characterTile, GameObject targetCharacter, bool simulate) {
        if(characterTile != null && targetCharacter != null)
        {
            TileScript tile = characterTile.GetComponent<TileScript>();
            Vector2Int targetPos = targetCharacter.GetComponent<Character>().gridPosition;
            if(tile.hasCharacter && IsAllyUnit(tile.characterOn) != IsAllyUnit(targetCharacter) && Mathf.Abs(tile.position.x - targetPos.x) + Mathf.Abs(tile.position.y - targetPos.y) == 1) //Need target on opposite team
            {
                if(!simulate)
                {
                    Debug.Log("Triggering combo attack...");
                    StartCoroutine(EndComboAttack(tile.characterOn, targetCharacter));
                }
                return true;
            }
        }
        return false;
    }

    IEnumerator EndComboAttack(GameObject user, GameObject target) {
        yield return new WaitWhile(() => !user.GetComponent<Character>().RotateTowards(target.GetComponent<Character>().GetTileObject().transform.position)); //Wait for rotation first
        GetComboAttack(user).AffectCharacter(user, target, 0);
        yield break;
    }

    //Try to move the unit to the specified position on the grid. Returns true if move succeeds. Will not affect game state if simulate is true.
    public bool MoveUnit(Vector2Int tilePos, bool simulate) 
    {
        var gridTiles = grid.GetComponent<Grid>();
        if(!moved && moving && charSelected && activeUnit == gridTiles.GetCharacterAtPos(selectedCharPos) && tilePos != selectedCharPos) 
        {
            // Move character
            if (gridTiles.PathCharacterOnTile(selectedCharPos, tilePos, true) == false)
            {
                Debug.Log("Cannot move character to tile " + tilePos.x + " " + tilePos.y);
                return false;
            }
            else 
            {
                if(simulate) return true;
                StartCoroutine(EndMoveUnit(tilePos));
                return true;
            }
        }
        return false;
    }

    IEnumerator EndMoveUnit(Vector2Int tilePos) {
        var gridTiles = grid.GetComponent<Grid>();
        // Unselect currently select character
        Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
        activeUnitPos = tilePos;
        activeUnitTile = gridTiles.GetTileAtPos(tilePos);
        gridTiles.Tiles[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = IsTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
        charSelected = false;
        charCard.Close();
        charHighlighted = false;
        ResetAllHighlights();
        moved = true;
        DisableCoins();
        foreach(GameObject unit in aliveUnits) unit.GetComponent<Character>().HideBars();
        yield return new WaitWhile(() => activeUnit.GetComponent<FollowPath>().PathToFollow.Count > 0 || activeUnit.GetComponent<FollowPath>().IsMoving());
        yield return new WaitForSecondsRealtime(0.15f);
        EnableCoins();
        if(!acted) {
            SelectAction(); //Move to action state if available
        }
        foreach(GameObject unit in aliveUnits) unit.GetComponent<Character>().ShowBars();
        LogicUpdate();
        yield break;
    }

    // --------------------------------------------------------------
    // @desc: Pause the battle engine for a given amount of time
    // @arg: waitTime - real time seconds to wait for
    // @arg: endTurnAfter - whether a new turn happens after the wait
    // --------------------------------------------------------------
    public IEnumerator PauseBattleEngine(float waitTime, bool endTurnAfter = false)
    {
        // Before wait
        active = false;
        interactable = false;

        DisableCoins();
        yield return new WaitForSecondsRealtime(waitTime);
        EnableCoins();

        // After wait
        active = true;
        interactable = true;
        LogicUpdate();

        if (endTurnAfter)
        {
            EndTurn();
        }
        yield break;
    }

    //Perform all end-of-turn logic
    public void LogicUpdate() {
        if(isPlayerTurn) RefreshActionButtons();
        //Collect any dead units
        foreach(GameObject unit in units) {
            if(!IsUnitAlive(unit)) {
                if(grid.GetComponent<Grid>().RemoveCharacter(unit)) {
                    deadUnits.Add(unit);
                    if(IsAllyUnit(unit)) {
                        AddPlayerMorale(-5);
                        AddEnemyMorale(2);
                    }
                    else {
                        AddEnemyMorale(-5);
                        AddPlayerMorale(2);
                    }
                }
            }
        }
        foreach(GameObject unit in deadUnits) aliveUnits.Remove(unit);
        playerShipBar.SetHealth(GetPlayerCrew().GetShip().hp);
        enemyShipBar.SetHealth(GetEnemyCrew().GetShip().hp);
        playerMoraleBar.SetHealth(GetPlayerCrew().morale);
        enemyMoraleBar.SetHealth(GetEnemyCrew().morale);
        UpdateTurnOrder();

        CheckOutcome();
        if((moved && acted) || deadUnits.Contains(activeUnit)) EndTurn();
        CheckOutcome();
    }

    public void AddPlayerMorale(int morale) {
        AddMorale(GetPlayerCrew(), morale);
    }

    public void AddEnemyMorale(int morale) {
        AddMorale(GetEnemyCrew(), morale);
    }

    private void AddMorale(Crew crew, int morale) {
        int lastMorale = crew.morale;
        crew.AddMorale(morale);
        if(crew.morale >= MoraleHigh) {
            if(lastMorale <= MoraleLow) crew.ClearModifiersWithId(MoraleDebuffID);
            else if(lastMorale < MoraleHigh) {
                foreach(StatModifier modifier in MoraleBuffs) crew.AddModifier(modifier);
            }
        }
        else if(crew.morale <= MoraleLow) {
            if(lastMorale <= MoraleHigh) crew.ClearModifiersWithId(MoraleBuffID);
            else if(lastMorale > MoraleLow) {
                foreach(StatModifier modifier in MoraleDebuffs) crew.AddModifier(modifier);
            }
        }
        else {
            if(lastMorale >= MoraleHigh) crew.ClearModifiersWithId(MoraleBuffID);
            else if(lastMorale <= MoraleLow) crew.ClearModifiersWithId(MoraleDebuffID);
        }
    }

    //Check whether victory or defeat conditions are met
    public void CheckOutcome() {
        CheckDefeat();
        CheckVictory();
    }

    //Check for victory conditions and end the battle if met
    private void CheckVictory() {
        bool won = true;
        //Check if any enemies are alive
        foreach(GameObject unit in aliveUnits) {
            if(!IsAllyUnit(unit)) {
                won = false;
                break;
            }
        }
        if(enemyCrew.GetComponent<Crew>().GetShip().hp <= 0) won = true;
        //End on win
        if(won) {
            OnEnd();
            victoryText.SetActive(true);
            victory = true;
        }
    }

    //Check for defeat conditions and end the battle if met
    private void CheckDefeat() {
        bool loss = true;
        //Check if any allies are alive
        foreach(GameObject unit in aliveUnits) {
            if(IsAllyUnit(unit)) {
                loss = false;
                break;
            }
        }
        if(playerCrew.GetComponent<Crew>().GetShip().hp <= 0) loss = true;
        //End on loss
        if(loss) {
            OnEnd();
            defeatText.SetActive(true);
            defeat = true;
        }
    }

    private void OnEnd() {
        active = false;
        DisableCoins();
        deadUnits.Clear();
    }

    //Surrender the battle for the acting crew
    public void Surrender() {
        if(isPlayerTurn) {
            defeatText.SetActive(true);
            defeat = true;
        }
        else {
            victoryText.SetActive(true);
            victory = true;
        }
        OnEnd();
        surrenderText.SetActive(true);
        surrendered = true;
    }

    public static bool IsUnitAlive(GameObject unit) {
        return !unit.GetComponent<Character>().IsDead();
    }

    public static bool IsAllyUnit(GameObject unit) {
        return unit.GetComponent<Character>().crew.GetComponent<Crew>().isPlayer;
    }

    public static Ability GetBasicAttack(GameObject unit) {
        return unit.GetComponent<Character>().basicAttack;
    }

    public static Ability GetComboAttack(GameObject unit) {
        return unit.GetComponent<Character>().comboAttack;
    }

    public Crew GetPlayerCrew() {
        return playerCrew.GetComponent<Crew>();
    }

    public Crew GetEnemyCrew() {
        return enemyCrew.GetComponent<Crew>();
    }

    public int GetTurnCount() {
        return turnCount;
    }

    public void RefreshActionButtons() {
        if(activeUnit == null) return;
        foreach(Button button in actionButtons) Destroy(button.gameObject);
        actionButtons.Clear();
        if(acted) return;

        int count = 0;
        var activeChar = activeUnit.GetComponent<Character>();
        //Setup action buttons
        foreach(Ability ability in activeChar.GetBattleAbilities()) {
            GameObject actionButton = Instantiate(buttonPrefab, canvas.transform);
            actionButton.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(70, -90 - 25 * count);
            Button button = actionButton.GetComponent<Button>();

            button.onClick.AddListener(() => { //Listen to setup ability when clicked
                selectedAbility = ability;
                SetupAction(activeUnitTile);
            });

            button.interactable = !usedAbilities.Contains(ability) && activeUnit.GetComponent<Character>().ap >= ability.costAP; //Only allow ability selection if it wasn't used already and AP is available
            var tmp = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if(!button.interactable) tmp.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            tmp.text = ability.displayName; //Set button name to ability name
            actionButtons.Add(button);
            count++;
        }

        //Setup interaction buttons
        var gridTiles = grid.GetComponent<Grid>();
        List<GameObject> tileObjects = gridTiles.GetInteractableTileObjects(activeUnitPos);
        Debug.Log("Adding actions for " + tileObjects.Count + " grid objects...");
        foreach(GameObject tileObject in tileObjects)
        {
            var gridObject = tileObject.GetComponent<CannonObject>();
            if(gridObject.interactionType == GridObject.InteractionType.None) continue;
            bool single = gridObject.interactionType != GridObject.InteractionType.Double;
            for(int i = 0; i < (single ? 1 : 2); i++)
            {
                GameObject actionButton = Instantiate(buttonPrefab, canvas.transform);
                actionButton.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(70, -90 - 25 * count);
                Button button = actionButton.GetComponent<Button>();

                if(i == 0)
                {
                    button.onClick.AddListener(() => { //Listen to setup ability when clicked
                        GameObject trackedObject = gridObject.InteractPrimary(activeUnit);
                        if(trackedObject != null) cam.GetComponent<CameraController>().SetCameraFollow(trackedObject);
                        acted = true;
                        StartCoroutine(PauseBattleEngine(1f, true));
                    });
                }
                else
                {
                    button.onClick.AddListener(() => { //Listen to setup ability when clicked
                        GameObject trackedObject = gridObject.InteractSecondary(activeUnit);
                        if(trackedObject != null) cam.GetComponent<CameraController>().SetCameraFollow(trackedObject);
                        acted = true;
                        StartCoroutine(PauseBattleEngine(1f, true));
                    });
                }

                var tmp = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if(!button.interactable) tmp.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                tmp.text = i == 0 ? gridObject.displayNamePrimary : gridObject.displayNameSecondary; //Set button name
                actionButtons.Add(button);
                count++;
            }
        }

        //Setup travel button
        var activeUnitTileScript = activeUnitTile.GetComponent<TileScript>();
        if(activeUnitTileScript.hasGridLink)
        {
            GameObject actionButton = Instantiate(buttonPrefab, canvas.transform);
            actionButton.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(70, -90 - 25 * count);
            Button button = actionButton.GetComponent<Button>();

            button.onClick.AddListener(() => { //Listen to setup ability when clicked
                gridTiles.MoveUnitToGrid(activeUnitTile);
                acted = true;
                LogicUpdate();
            });

            var tmp = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if(!button.interactable) tmp.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            tmp.text = "Travel";
            actionButtons.Add(button);
            count++;
        }
    }

    public void EnableCoins() {
        if(!active) return;
        if(!acted) actionCoinAnimator.SetBool(Interactable, true);
        if(!moved) moveCoinAnimator.SetBool(Interactable, true);
        endCoinAnimator.SetBool(Interactable, true);
        surrenderCoinAnimator.SetBool(Interactable, true);
    }

    public void DisableCoins() {
        actionCoinAnimator.SetBool(Interactable, false);
        moveCoinAnimator.SetBool(Interactable, false);
        endCoinAnimator.SetBool(Interactable, false);
        surrenderCoinAnimator.SetBool(Interactable, false);
    }

    public void SelectCoin(GameObject coin, Animator animator, bool value) {
        animator.SetBool(Selected, value);
        coin.transform.GetChild(0).gameObject.SetActive(value); //Set text
    }

    public void ShowActionsList() {
        foreach(Button button in actionButtons) {
            button.gameObject.SetActive(true);
        }
    }

    public void HideActionsList() {
        foreach(Button button in actionButtons) {
            button.gameObject.SetActive(false);
        }
    }

    public bool HasMoved() {
        return moved;
    }

    public bool HasActed() {
        return acted;
    }

    public void DoAITurn() 
    {
        Debug.Log("doAITurn: AI Turn began");

        //StartCoroutine(aiTurnWaiter());
        StartCoroutine(AIController.PerformTurn(activeUnit.GetComponent<Character>(), 2.0f, 2.0f));
    }

    IEnumerator AITurnWaiter()
    {
        //Debug.Log("AI: AI simple waiting");

        //Initalize variables for choosing random actions later
        int chosenAbility = 0;
        int chosenTarget = 0;

        //Initialize the target of the AI action variable
        TileScript actionTarget = null;

        //Get the grid script
        var gridScript = grid.GetComponent<Grid>();

        //Initialize variables based on the current character
        var activeChar = activeUnit.GetComponent<Character>();
        List<Ability> allAbilities = activeChar.GetBattleAbilities();

        //This is a list of lists of Vectors, used for storing the positions of possible targets for each ability.
        List<List<TileScript>> abilityTargetsLists = new List<List<TileScript>>();

        // Debug.Log("@@@@Current character class: " + activeChar.classname.ToString() 
        // + " || character at: " + activeUnitTile.GetComponent<TileScript>().position.x + ", " + activeUnitTile.GetComponent<TileScript>().position.y + "@@@@");

        //Iterate through the abilities of the character. For each ability, we'll keep a list of possible targets
        foreach(Ability selectedAbility in allAbilities)
        {
            //This is a temporary list of Vectors, used for storing the results of the targetAcquisitionTree to be added to the abilityTargetsLists lists
            List<TileScript> tempTiles = new List<TileScript>();

            PathTreeNode tempPathTree = new PathTreeNode();

            // Debug.Log("####Testing ability " + selectedAbility.displayName + " (friendly: " + selectedAbility.friendly.ToString() + ")"
            // + " with range " + selectedAbility.range 
            // + ". Character movement range " + activeChar.getMovement() 
            // + ", Total Range " + (activeChar.getMovement() + selectedAbility.range) + "####");

            //Retrieve the pathtreenode of the total range for this ability (character movement + ability range)
            tempPathTree = gridScript.GetAllPathsFromTile(activeUnitTile, (activeChar.GetMovement() + selectedAbility.range), true);

            //If the ability that is currently selected is marked for use on friendlies, that means for enemies it is supposed to be used for their opponents.
            if(selectedAbility.friendly == true)
            {
                //Debug.Log("Ability is friendly");

                TargetAcquisitionTree(tempPathTree, tempTiles, false);

                abilityTargetsLists.Add(tempTiles);
            }
            //Otherwise, the ability is one that targets enemies, or in this case is a 'friendly' ability
            else if(selectedAbility.friendly == false)
            {
                //Debug.Log("Ability is not friendly");

                TargetAcquisitionTree(tempPathTree, tempTiles, true);

                abilityTargetsLists.Add(tempTiles);
            }
        }

        //Check the list of targets, as there's a possibility the AI has no targets
        foreach(List<TileScript> subList in abilityTargetsLists)
        {
            //If there's at least a single target available for any ability
            if(subList.Count > 0)
            {
                //Leave this loop
                break;
            }
            //If there are no targets available
            else
            {
                //End the turn for now
                yield return new WaitForSecondsRealtime(2);
                EndTurn();
                yield break;
            }
        }

        //Choose a random ability from the possible abilities
        Debug.Log("AI: " +  activeChar.className.ToString()
        + " on (" + activeUnitPos.x + ", " + activeUnitPos.y
        + ") is choosing abilities between 0 and " + (abilityTargetsLists.Count - 1));
        
        //chosenAbility = Random.Range(0, (abilityTargetsLists.Count - 1));
        chosenAbility = 0;
        Debug.Log("AI: Chosen ability is ability " + chosenAbility + ", which is " + allAbilities[chosenAbility].displayName);

        //Set the BattleEngine's selected ability to the randomly chosen ability
        selectedAbility = allAbilities[chosenAbility];

        //If the random ability has no targets, choose again
        if(abilityTargetsLists[chosenAbility].Count == 0)
        {
            while(abilityTargetsLists[chosenAbility].Count == 0)
            {
                Debug.Log("AI: Chosen Ability has no targets. Reselecting...");
                Debug.Log("AI: Choosing abilities between 0 and " + (abilityTargetsLists.Count - 1));
                chosenAbility = Random.Range(0, (abilityTargetsLists.Count - 1));
                Debug.Log("AI: Chosen ability is ability " + chosenAbility + ", which is " + allAbilities[chosenAbility].displayName);
            }
        }

        //Go into the chosen ability list, and choose a target for that ability
        Debug.Log("AI: " +  activeChar.className.ToString()
        + " on (" + activeUnitPos.x + ", " + activeUnitPos.y
        + ") is choosing targets between 0 and " + (abilityTargetsLists[chosenAbility].Count - 1));

        chosenTarget = Random.Range(0, (abilityTargetsLists[chosenAbility].Count - 1));

        Debug.Log("AI: Chosen target is number " + chosenTarget 
        + " which is class " + abilityTargetsLists[chosenAbility][chosenTarget].characterOn.GetComponent<Character>().className.ToString());

        //Set the final target based on the random selections
        actionTarget = abilityTargetsLists[chosenAbility][chosenTarget];
        Debug.Log("AI: Final target is " + actionTarget.characterOn.GetComponent<Character>().className.ToString() 
        + " at (" + actionTarget.position.x + ", " + actionTarget.position.y + ")");
        
        //If the character is self-targeting
        if(actionTarget.characterOn == activeUnit)
        {
            //End the turn and break out of the subroutine
            Debug.Log("AI: Self-target turn, ending");
            yield return new WaitForSecondsRealtime(2);
            EndTurn();
            yield break;
        }

        //Initialize a temporary PathTreeNode for approaching the target
        List<PathTreeNode> tempPathList = actionTarget.PathRef.PathToRootList();

        //Index for logging
        int logIndex = 0;

        //Log the path to the target
        foreach(PathTreeNode listNode in tempPathList)
        {
            var tempTileScript = listNode.MyTile.GetComponent<TileScript>();

            logIndex++;

            Debug.Log("AI: Range is : " + (activeChar.GetMovement() + allAbilities[chosenAbility].range)
            + " || Active Character is at (" + activeUnitPos.x + ", " + activeUnitPos.y 
            + ") || target is at (" + tempTileScript.position.x + ", " + tempTileScript.position.y + ")"
            + " || Count is " + logIndex);
        }

        //Initialize the TileScript that is directly adjacent to the target, where we will be trying to head towards
        TileScript adjacentTileScript = tempPathList[1].MyTile.GetComponent<TileScript>();
        Debug.Log("AI: adjacentTileScript: (" + adjacentTileScript.position.x + ", " + adjacentTileScript.position.y + ")");

        //Head towards the tile adjacent to the target
        Debug.Log("AI: Attempting move from (" + activeUnitPos.x + ", " + activeUnitPos.y + ")");
        var dummy = gridScript.MoveTowardsTile(activeUnitPos, adjacentTileScript.position, true, activeChar.GetMovement());
        

        //Update activeUnitPos to new gridPosition
        activeUnitPos = activeChar.gridPosition;
        Debug.Log("AI: Move attempted. New position is (" + activeUnitPos.x + ", " + activeUnitPos.y + ")");
        moving = false;

        //Attempt to use the ability on the target now that we've moved closer.
        Debug.Log("AI: Attempting to use ability " + selectedAbility.displayName 
        +  " from (" + activeUnitPos.x + ", " + activeUnitPos.y + ")");
        Debug.Log("AI: Ability was used: " + AIActUnit(actionTarget.position, false).ToString());

        //while(!Input.GetKeyDown(KeyCode.Space)) yield return null;
        yield return new WaitForSecondsRealtime(2);
        EndTurn();
    }
    
    public void TargetAcquisitionTree(PathTreeNode root, List<TileScript> returnList, bool validTarget) 
    {
        // Get data from tile
        var tileScript = root.MyTile.GetComponent<TileScript>();

        // if(tileScript.characterOn != null)
        // {
        //     Debug.Log("****Acquiring targets. Character at: " + tileScript.position.x + ", " + tileScript.position.y
        //     + " || List redundancy: " + (!returnList.Contains(tileScript)).ToString()
        //     + " || Character isPlayer: " + tileScript.characterOn.GetComponent<CharacterStats>().isPlayer().ToString() 
        //     + " || validTarget passed in: " + validTarget
        //     + " || target is fully valid: " + (tileScript.characterOn.GetComponent<CharacterStats>().isPlayer() == validTarget).ToString() + "****");
        // }

        //If there's a character on the tile, the list doesn't already have this tile, and the target is valid
        if(tileScript.characterOn != null && !returnList.Contains(tileScript) && tileScript.characterOn.GetComponent<Character>().IsPlayer() == validTarget)
        {
            //add this tile to the list of possible targets
            returnList.Add(tileScript);

            // Debug.Log("////Adding target on grid " + tileScript.position.x + ", " + tileScript.position.y 
            // + " with character class " + tileScript.characterOn.GetComponent<CharacterStats>().classname.ToString() 
            // + " with isPlayer " + tileScript.characterOn.GetComponent<CharacterStats>().isPlayer() + "////");
        }

        //recurse through all tiles in the tree
        if(root.Up != null) TargetAcquisitionTree(root.Up, returnList, validTarget);
        if(root.Down != null) TargetAcquisitionTree(root.Down, returnList, validTarget);
        if(root.Left != null) TargetAcquisitionTree(root.Left, returnList, validTarget);
        if(root.Right != null) TargetAcquisitionTree(root.Right, returnList, validTarget);
    }

    public bool AllianceChecker(GameObject unit) 
    {
        return unit.GetComponent<Character>().IsPlayer() == activeUnit.GetComponent<Character>().IsPlayer();
    }

    //Try to use the selected ability at the specified position on the grid. Returns true if action succeeds. Will not affect game state if simulate is true.
    public bool AIActUnit(Vector2Int tilePos, bool simulate) 
    {
        Debug.Log("Moving: " + moving.ToString()
        + " || Acted: " + acted.ToString()
        + " || Selected Ability: " + selectedAbility.displayName 
        + " || activeUnit AP: " + activeUnit.GetComponent<Character>().ap
        + " || Selected Ability Cost: " + selectedAbility.costAP);

        // if(moving || acted || selectedAbility == null || activeUnit.GetComponent<CharacterStats>().AP < selectedAbility.costAP)
        // {
        //     return false;
        // }
        
        var gridTiles = grid.GetComponent<Grid>();
        var tileScript = gridTiles.GetTileAtPos(tilePos).GetComponent<TileScript>();

        int dist = Mathf.Abs(activeUnitPos.x - tilePos.x) + Mathf.Abs(activeUnitPos.y - tilePos.y); //Take Manhattan distance

        if(dist > selectedAbility.range)
        {
            return false;  
        } 

        Debug.Log("Past Manhattan distance");

        GameObject selectedCharacter = null;

        if(selectedAbility.requiresTarget) 
        { 
            // Debug.Log("Selected Target Check");
            // //Check for valid target
            // if(!tileScript.hasCharacter)
            // { 
            //     return false;
            // }
            // if(selectedAbility.friendly && !allianceChecker(gridTiles.GetCharacterAtPos(tilePos)))
            // {
            //     return false;
            // } 
            // if(!selectedAbility.friendly && allianceChecker(gridTiles.GetCharacterAtPos(tilePos)))
            // {
            //     return false;
            // }

            selectedCharacter = tileScript.characterOn;
        }


        if(simulate) return true;

        int xDist = activeUnitPos.x - tilePos.x;
        int yDist = activeUnitPos.y - tilePos.y;

        List<GameObject> characters = new List<GameObject>();

        //Select characters based on ability shape
        foreach(Vector2Int pos in selectedAbility.GetRelativeShape(xDist, yDist))
        {
            Vector2Int selPos = new Vector2Int(tilePos.x + pos.x, tilePos.y + pos.y);
            var selTile = gridTiles.GetTileAtPos(selPos);
            if(selTile == null) continue;
            var selTileScript = selTile.GetComponent<TileScript>();
            if(selTileScript.hasCharacter)
            {
                if(selectedAbility.friendly && !AllianceChecker(gridTiles.GetCharacterAtPos(selPos)))
                {
                    continue;
                }
                if(!selectedAbility.friendly && AllianceChecker(gridTiles.GetCharacterAtPos(selPos)))
                {
                    continue;   
                } 
                characters.Add(selTileScript.characterOn);
            }
        }

        if(!selectedAbility.free)
        {
            acted = true;
        }
        activeUnit.GetComponent<Character>().AddAP(-selectedAbility.costAP);
        usedAbilities.Add(selectedAbility);

        StartCoroutine(AIEndActUnit(selectedCharacter == null ? tilePos : selectedCharacter.GetComponent<Character>().gridPosition, characters, xDist, yDist));
        return true;
    }

    IEnumerator AIEndActUnit(Vector2Int tilePos, List<GameObject> characters, int xDist, int yDist) {
        DisableCoins();
        foreach(GameObject unit in aliveUnits)
        {
            if(unit != activeUnit && !characters.Contains(unit)) 
            {
                unit.GetComponent<Character>().HideBars();
            }
        }
        foreach(Button button in actionButtons)
        {
            button.interactable = false;
            button.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        if(tilePos != activeUnitPos) yield return new WaitWhile(() => !activeUnit.GetComponent<Character>().RotateTowards(grid.GetComponent<Grid>().GetTileAtPos(tilePos).transform.position)); //Wait for rotation first

        var gridTiles = grid.GetComponent<Grid>();
        //Pull and knockback
        // if(selectedAbility.knockback != 0)
        // {
        //     foreach(GameObject character in characters) selectedAbility.applyKnockback(character.GetComponent<CharacterStats>(), gridTiles, xDist, yDist);
        // }
        // //Self movement
        // Vector2Int newPos = selectedAbility.applySelfMovement(activeUnit.GetComponent<CharacterStats>(), gridTiles, xDist, yDist);
        // if(newPos != activeUnitPos)
        // {
        //     activeUnitPos = newPos;
        //     activeUnitTile = gridTiles.GetTileAtPos(newPos);
        //     gridTiles.grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = isTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
        //     selectedCharPos = newPos;
        // }
        ResetAllHighlights();
        //if(!acted) highlightActionTiles(newPos, selectedAbility.range);
        for(int i = 0; i < selectedAbility.totalHits; i++)
        {
            selectedAbility.AffectCharacters(activeUnit, characters, i);
            yield return new WaitForSecondsRealtime(0.6f);
        }

        //AI: Forward the target(s) to AI handler for enqueue. Currently only forwards one character - for refactoring later
        //if(characters.Count > 0) playerTarget = characters[0];

        // ----- Combo Attacks ------
        if(TryComboAttack(activeUnitPos, tilePos, false)) yield return new WaitForSecondsRealtime(0.6f);
        // --------------------------

        foreach(GameObject unit in aliveUnits) unit.GetComponent<Character>().ShowBars();
        //enableCoins();

        LogicUpdate();
        yield break;
    }

}
