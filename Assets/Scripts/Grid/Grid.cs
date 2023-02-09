/// @author: Bryson Squibb
/// @date: 10/08/2022
/// @description: this script spawns a grid of tile objects, then
/// it provides an interface to interact with said spawned grid

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Direction
{
    public static readonly Direction NORTH = new(0, 1);
    public static readonly Direction SOUTH = new(0, -1);
    public static readonly Direction EAST = new(1, 0);
    public static readonly Direction WEST = new(-1, 0);
    public readonly int xMove, yMove;

    private Direction(int xMove, int yMove)
    {
        this.xMove = xMove;
        this.yMove = yMove;
    }
}

public class Grid : MonoBehaviour
{
    [SerializeField] public Grid cabinGrid; // GridRef
    [SerializeField] public GameObject linkBox; // Used as a linking visual
    [SerializeField] public int gridNum; // Grid number 
    [SerializeField] public int tilesize; // Size of each tile
    [SerializeField] public int gridLayer; // Layer used for camera culling

    // Grid vars
    [FormerlySerializedAs("Tile")] public GameObject tile; // A cell or tile that makes up the grid
    public GameObject [,] Tiles; // The actual grid, represented by a 2d array
    private uint availableTiles;
    public uint height, width;

    // Things used for click detection
    private Camera cam;

    // Materials
    [SerializeField] public Material unselected, highlighted, selected,
                                     activeUnselected, activeHighlighted, activeSelected,
                                     ability, abilityHighlighted, impassible;

    // Crews
    public List<GameObject> crews;
    
    // Awake is called before Start()
    void Awake()
    {
        // Generate the grid by instantiating tile objects
        GenerateGrid(gridNum);

        // Get camera
        cam = Camera.main;

        // Spawn characters (random positions)
        foreach(GameObject crew in crews) {
            crew.GetComponent<Crew>().InitCharacters();
            List<GameObject> characters = crew.GetComponent<Crew>().characters;
            foreach(GameObject character in characters) {
                SpawnCharacter(character);
            }
        }
    }

    void Start()
    {
        // Create links
        if (cabinGrid != null)
        {
            CreateLinkTile(new Vector2Int(3,25), new Vector2Int(3,9), cabinGrid);
            CreateLinkTile(new Vector2Int(4,25), new Vector2Int(4,9), cabinGrid);
        }
    }

    // --------------------------------------------------------------
    // @desc: Create a grid object for every tile position
    // --------------------------------------------------------------
    void GenerateGrid(int gridNumber = 0)
    {
        // Set Grid Map
        GridMap passableTiles = new GridMap(gridNumber);

        // Set dimensions
        width = passableTiles.GetGridWidth();
        height = passableTiles.GetGridHeight();

        // Declare the grid to appropriate width and height
        Tiles = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position of the tile to create
                Vector2Int xy = new Vector2Int(x, y);
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y + passableTiles.GetHeightOffsetAtPos(xy), transform.position.z + y*tilesize);
                
                // Create the tile
                GameObject newTile = Instantiate(tile, pos, transform.rotation, this.transform);

                // Set the data of the newly created tile
                newTile.name = "Tile " + x + " " + y;
                var newTileScript = newTile.GetComponent<Tile>();
                newTileScript.position.x = x;
                newTileScript.position.y = y;
                newTileScript.myLayer = gridLayer;
                newTileScript.grid = this;

                // Check the grid map for impassible tiles
                if (passableTiles.GetFlagAtPos(xy))
                {
                    SetTileToImpassible(newTile);
                }

                // Link the new tile to a position on the grid
                Tiles[x, y] = newTile;
            }
        }

        // Tiles valid for spawning characters
        availableTiles = width*height;
    }
    
    // --------------------------------------------------------------
    // @desc: Spawn a character on the grid
    // @arg: character - the character prefab object to spawn
    // @arg: newCharacter - whether we need to spawn this character
    //  we can turn this off for attaching characters instead
    // --------------------------------------------------------------
    public bool SpawnCharacter(GameObject character, bool newCharacter = true)
    {
        GameObject spawningTile;
        bool characterSpawned = false;
        int randX;
        int randY;
        
        // Check if there's a tile available
        if (availableTiles > 0)
        {
            // Warning: potential issue if the grid is full + performance issue if
            // grid is almost full
            while (characterSpawned == false)
            {
                // Get a random tile on the grid
                randX = Random.Range(0, (int)width);
                randY = Random.Range(0, (int)height);
                spawningTile = Tiles[randX, randY];

                // Get data from tile object
                var tilesScript = spawningTile.GetComponent<Tile>();

            // Check whether the tile already has a character on it
            // + whether the tile is a passable tile
            if (tilesScript.hasCharacter == false && tilesScript.passable)
                {
                    // Spawn the character if valid
                    Vector3 tilePos = spawningTile.transform.position;
                    Vector3 pos = new Vector3(tilePos.x, tilePos.y+0.5f, tilePos.z);
                    Debug.Log("Spawning character as position " + randX + " " + randY);

                    // Set tile data
                    if (newCharacter)
                    {
                        tilesScript.characterOn = Instantiate(character, pos, transform.rotation, this.transform);
                    }
                    else
                    {
                        // Set Tilescript Character
                        tilesScript.characterOn = character;

                        // Modify Character Transformation
                        character.transform.position = pos;
                        character.transform.rotation = transform.rotation;
                        character.transform.parent = this.transform;
                    }

                    tilesScript.hasCharacter = true;
                    tilesScript.characterOn.GetComponent<Animator>().Play(0, -1, Random.value); //Randomize start frame
                    tilesScript.characterOn.transform.GetChild(0).gameObject.layer = gridLayer;
                    tilesScript.characterOn.GetComponent<Character>().gridPosition = new Vector2Int(randX, randY);
                    tilesScript.characterOn.GetComponent<Character>().myGrid = this.gameObject;
                    tilesScript.characterOn.layer = gridLayer;

                    // Update flag and # of available tiles
                    availableTiles--;
                    characterSpawned = true;

                }
            }
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Couldn't spawn character since there are no available tiles");
        }

        return characterSpawned;
    }

    // --------------------------------------------------------------
    // @desc: Spawn a character on the grid
    // @arg: character - the character prefab object to spawn
    // @arg: spawnPos  - the logical grid position to spawn on
    // --------------------------------------------------------------
    public bool SpawnCharacter(GameObject character, Vector2Int spawnPos, bool newCharacter = true)
    {
        GameObject spawningTile;
        bool characterSpawned = false;
        
        // Get the respective tile on the grid
        spawningTile = Tiles[spawnPos.x, spawnPos.y];

        // Get data from tile object
        var tilesScript = spawningTile.GetComponent<Tile>();

        // Check whether the tile already has a character on it
        // + whether the tile is a passable tile
        if (tilesScript.hasCharacter == false && tilesScript.passable)
        {
            // Spawn the character if valid
            Vector3 tilePos = spawningTile.transform.position;
            Vector3 pos = new Vector3(tilePos.x, tilePos.y+0.5f, tilePos.z);
            Debug.Log("Spawning character as position " + spawnPos.x + " " + spawnPos.y);
            character.GetComponent<Character>().gridPosition = spawnPos;
            character.GetComponent<Character>().myGrid = this.gameObject;
            character.transform.GetChild(0).gameObject.layer = gridLayer;

            // Set tile data
            if (newCharacter)
            {
                tilesScript.characterOn = Instantiate(character, pos, transform.rotation, this.transform);
            }
            else
            {
                // Set Tilescript Character
                tilesScript.characterOn = character;

                // Modify Character Transformation
                character.transform.position = pos;
                character.transform.rotation = transform.rotation;
                character.transform.parent = this.transform;
            }

            tilesScript.characterOn.GetComponent<Animator>().Play(0, -1, Random.value); //Randomize start frame

            tilesScript.hasCharacter = true;
            character.layer = gridLayer;

            // Update flag and # of available tiles
            availableTiles--;

            // Whether the character has spawned
            characterSpawned = true;
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Can't spawn a character on top of another character");
        }

        return characterSpawned;
    }

    // --------------------------------------------------------------
    // @desc: Attach an object to the grid
    // @arg: obj     - the object to attach
    // @arg: gridPos - the logical grid position to move the object to
    // @arg: overlapCharacter - whether this object can overlap a character
    // @ret: bool    - whether this method was successful or not
    // --------------------------------------------------------------
    public bool AttachObjectToGrid(GameObject obj, Vector2Int gridPos, bool overlapCharacter = false)
    {
        GameObject tile;
        bool objectAttached = false;
        bool isValid = true;

        // Get the respective tile on the grid
        tile = Tiles[gridPos.x, gridPos.y];
        Debug.Log("Attaching object to position " + gridPos.x + " " + gridPos.y);

        // Get data from tile object
        var tilesScript = tile.GetComponent<Tile>();
    
        // Check range
        if (TilePosInRange(gridPos))
        {
            // If this object cannot overlap a character, then check whether
            // this tile has a character on it
            if (overlapCharacter == false && tilesScript.hasCharacter == true)
            {
                isValid = false;
            }
        }
        
        // If the object can spawn
        if (isValid)
        {
            // Get tile pos
            Vector3 tilePos = tile.transform.position;

            // Move object to tile pos
            obj.transform.parent   = tile.transform;
            obj.transform.position = tilePos;

            // Attach object to tile
            tilesScript.objectOn = obj;
            tilesScript.hasObject = true;

            // Set impasssible if applicable
            if (overlapCharacter == false)
            {
                SetTileToImpassible(tile);
            }

            // Set flag
            objectAttached = true;
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Object cannot spawn here");
        }

        return objectAttached;
    }

    // --------------------------------------------------------------

    public bool RemoveCharacter(Character character) {
        Vector2Int tilePos = character.GetComponent<Character>().gridPosition;
        var tile = GetTileAtPos(tilePos).GetComponent<Tile>();
        if(tile.characterOn == character.gameObject) {
            character.RemoveFromGrid();
            tile.characterOn = null;
            tile.hasCharacter = false;
            return true;
        }
        return false;
    }

    // Move character in a straight line from source to destination
    public bool GetLinearPath(Vector2Int sourcePos, Vector2Int destPos) {
        GameObject charToMove = null;

        // Check whether tiles are in range
        if (TilePosInRange(sourcePos) && TilePosInRange(destPos))
        {
            // Get tile on source position
            GameObject sourceTile = Tiles[sourcePos.x, sourcePos.y];
            var sourceTileScript = sourceTile.GetComponent<Tile>();

            // Get tile on dest position
            GameObject destTile = Tiles[destPos.x, destPos.y];
            var destTileScript = destTile.GetComponent<Tile>();

            Vector2Int path = new Vector2Int(0, 0);
            if(!sourceTileScript.hasCharacter) return false;
            int xDist = destPos.x - sourcePos.x;
            int yDist = destPos.y - sourcePos.y;
            if(xDist > 0) {
                for(int i = 1; i <= xDist; i++) {
                    var tileScript = Tiles[sourcePos.x + i, sourcePos.y].GetComponent<Tile>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(i, 0);
                }
            }
            else {
                for(int i = -1; i >= xDist; i--) {
                    var tileScript = Tiles[sourcePos.x + i, sourcePos.y].GetComponent<Tile>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(i, 0);
                }
            }
            if(yDist > 0) {
                for(int i = 1; i <= yDist; i++) {
                    var tileScript = Tiles[sourcePos.x, sourcePos.y + i].GetComponent<Tile>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(0, i);
                }
            }
            else {
                for(int i = -1; i >= yDist; i--) {
                    var tileScript = Tiles[sourcePos.x, sourcePos.y + i].GetComponent<Tile>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(0, i);
                }
            }

            if(path.x != 0 || path.y != 0) {
                charToMove = sourceTileScript.characterOn;
                var pos = new Vector2Int(sourcePos.x + path.x, sourcePos.y + path.y);
                var tileScript = GetTileAtPos(pos).GetComponent<Tile>();

                charToMove.GetComponent<FollowPath>().abilityDist = path;

                // Move camera to destPos
                cam.GetComponent<CameraController>().SetCameraFollow(charToMove);

                // Set source tile data
                sourceTileScript.hasCharacter = false;
                sourceTileScript.characterOn = null;

                // Set destination tile data
                tileScript.hasCharacter = true;
                tileScript.characterOn = charToMove;

                charToMove.GetComponent<Character>().gridPosition = pos;

                return true;
            }
        }
        else
        {
            Debug.Log("GetLinearPath: Error! tile source or dest position is out of range");
        }
        
        return false;
    }

    // --------------------------------------------------------------
    // @desc: Move a character along an automatically generated path
    // @arg: sourceTile - the source tile script with a character on it
    // @arg: destTile   - the destination tile script to move the character to
    // @arg: maxRange   - the maximum range the character can move
    // @ret: bool       - whether the move is successful or not
    // --------------------------------------------------------------
    public bool MoveTowardsTile(Tile sourceTile, Tile destTile, bool onlyHighlighted, int maxRange)
    {
        GameObject charToMove = null;
        bool moveSuccess = false;
        Vector2Int sourcePos = sourceTile.position;
        Vector2Int destPos = destTile.position;

        // Check whether tiles are in range
        if (TilePosInRange(sourcePos) && TilePosInRange(destPos))
        {
            // Make a new path tree (Warning: we must reach the destination tile for this to work.
            // Right now this is achieved by making the max range huge, but it's not efficient)
            sourceTile.PathRef = GetAllPathsFromTile(sourceTile.gameObject, maxRange+100);


            // Get character on source tile
            if (sourceTile.hasCharacter && !destTile.hasCharacter && destTile.passable)
            {
                // Only move to highlighted tiles
                if (!onlyHighlighted || destTile.highlighted)
                {
                    Debug.Log("Moving character to tile " + destPos.x + " " + destPos.y);
                    charToMove = sourceTile.characterOn;

                    // Get a list of tiles towards the destination
                    List<PathTreeNode> pathTowardsDest = destTile.PathRef.PathToRootList();

                    // Cut down stack to max range
                    while (pathTowardsDest.Count-1 > maxRange || destTile.hasCharacter)
                    {
                        pathTowardsDest.RemoveAt(0);

                        // Destination tile is the first element in the list
                        destTile = pathTowardsDest[0].MyTile.GetComponent<Tile>();
                    }
                    
                    // Move character to the new destination, instead
                    destTile.PathRef.PathToRootOnStack(charToMove.GetComponent<FollowPath>().PathToFollow);

                    // Set camera to follow moving character
                    cam.GetComponent<CameraController>().SetCameraFollow(charToMove);

                    // Set source tile data
                    sourceTile.hasCharacter = false;
                    sourceTile.characterOn = null;

                    // Set destination tile data
                    destTile.hasCharacter = true;
                    destTile.characterOn = charToMove;

                    // Update character data
                    Character charScript = charToMove.GetComponent<Character>();
                    charScript.gridPosition = destTile.position;
                    charScript.myGrid = destTile.grid.gameObject;
                    charScript.transform.SetParent(charScript.myGrid.transform.parent.transform, true);

                    moveSuccess = true;
                }
                else
                {
                    Debug.Log("MoveTowardsTile: Error! cannot move to unhighlighted tiles");
                }
            }
            else
            {
                Debug.Log("MoveTowardsTile: Error! source tile does not have a character");
            }
        }
        else
        {
            Debug.Log("MoveTowardsTile: Error! tile source or dest position is out of range");
        }
        
        return moveSuccess;
    }

    // --------------------------------------------------------------
    // @desc: Makes a tile impassible
    // @arg: GameObject - tile to make impassible
    // --------------------------------------------------------------
    public void SetTileToImpassible(GameObject tile)
    {
        // Get script from tile
        var tileScript = tile.GetComponent<Tile>();

        // Set tile to impassible
        tileScript.passable = false;

        // Change material
        tile.GetComponent<Renderer>().material = impassible;

        // Update available tiles
        availableTiles--;
    }

    // --------------------------------------------------------------
    // @desc: Designate a tile from a tile pos to link to another
    // grid so characters can travel to it
    // @arg: sourceTilePos - logical grid position of the source tile
    // @arg: destTilePos   - logical grid position of the destination tile
    // @arg: targetGridScript - the target grid to link the source tile to
    // @ret: bool - whether the link was successful or not
    // --------------------------------------------------------------
    public bool CreateLinkTile(Vector2Int sourceTilePos, Vector2Int destTilePos, Grid targetGridScript)
    {
        bool linkSuccess = false;

        // Get tiles from positions
        GameObject sourceTile = GetTileAtPos(sourceTilePos);
        GameObject destTile   = targetGridScript.GetTileAtPos(destTilePos);

        // Check get
        if (sourceTile != null && destTile != null)
        {
            // Get tile scripts
            var sourceScript = sourceTile.GetComponent<Tile>();
            var destScript = destTile.GetComponent<Tile>();

            // Spawn boxes for visual element
            GameObject sourceBox = Instantiate(linkBox);
            GameObject destBox = Instantiate(linkBox);
            AttachObjectToGrid(sourceBox, sourceTilePos, true);
            targetGridScript.AttachObjectToGrid(destBox, destTilePos, true);

            // Set layers
            sourceBox.transform.GetChild(0).gameObject.layer = gridLayer;
            destBox.transform.GetChild(0).gameObject.layer = targetGridScript.gridLayer;

            // Set links
            sourceScript.hasGridLink = true;
            sourceScript.targetGrid = targetGridScript;
            sourceScript.targetTile = destScript;
            destScript.hasGridLink = true;
            destScript.targetGrid = this;
            destScript.targetTile = sourceScript;

            // Set flag
            linkSuccess = true;
        }
        else
        {
            Debug.Log("CreateLinkTile: Error! could not get source and destination tiles");
        }

        return linkSuccess;
    }

    // --------------------------------------------------------------
    // @desc: Checks whether a logical tile position is in the grid
    // @arg: pos  - the position to check
    // @ret: bool - whether the position is in the grid
    // --------------------------------------------------------------
    private bool TilePosInRange(Vector2Int pos)
    {
        bool inRange = false;

        if (pos.x >= 0 && pos.x <= width-1)
        {
            if (pos.y >= 0 && pos.y <= height-1)
            {
                inRange = true;
            }
        }

        return inRange;
    }

    public PathTreeNode GetAllPathsFromTile(GameObject tile, int range, bool passThrough = false)
    {
        /*
        // Attempt to get boarding grids from the battle engine
        List<Grid> boardingGrids = null;
        BattleEngine battleScript = null;
        GameObject be = GameObject.FindWithTag("GameController");

        if (be.name == "BattleEngine")
        {
            battleScript = be.GetComponent<BattleEngine>();
            boardingGrids = battleScript.boardableGrids;
        }
        else
        {
            boardingGrids = new List<Grid>();
        }
        */

        // Call the real method from here
        return GetAllPathsFromTile(tile, new List<Grid>(), range, passThrough);
    }

    // --------------------------------------------------------------
    // @desc: Creates a tree data structure based on what moves a 
    // character can take from a given tile position
    // @arg: pos          - the root tile position
    // @arg: range        - how many tiles the character can move
    // @arg: passThrough  - makes every tile valid
    // @ret: PathTreeNode - the constructed tree
    // --------------------------------------------------------------
    public PathTreeNode GetAllPathsFromTile(GameObject tileObject, List<Grid> boardingGrids, int range, bool passThrough = false)
    {
        // Reset highlights beforehand
        ResetAllHighlights();

        var startTile = tileObject.GetComponent<Tile>();
        // Create root node
        PathTreeNode root = new PathTreeNode();
        root.MyTile = tileObject;
        root.TileRange = range;

        bool isPlayer = false;

        if (startTile.characterOn != null)
        {
            Character tileChar = startTile.characterOn.GetComponent<Character>();
            isPlayer = tileChar.IsPlayer();
        }

        // Temp vars
        PathTreeNode tempNode;

        // Create queue
        Queue<PathTreeNode> queue = new Queue<PathTreeNode>();
        queue.Enqueue(root);

        // Populate the tree based on the range value
        // this must be done in levelorder traversal
        while (queue.Count != 0)
        {
            // Highlight all nodes in the queue
            tempNode = queue.Dequeue();

            // Get data from tile
            var tileScript = tempNode.MyTile.GetComponent<Tile>();

            // Highlight tile
            int nDist = 1, sDist = 1, wDist = 1, eDist = 1;

            // Get neighboring tiles
            GameObject upTile = tileScript.grid.GetNeighboringTile(tileScript, isPlayer, passThrough, tempNode.TileRange, Direction.NORTH, boardingGrids, ref nDist);
            GameObject downTile = tileScript.grid.GetNeighboringTile(tileScript, isPlayer, passThrough, tempNode.TileRange, Direction.SOUTH, boardingGrids, ref sDist);
            GameObject leftTile = tileScript.grid.GetNeighboringTile(tileScript, isPlayer, passThrough, tempNode.TileRange, Direction.WEST, boardingGrids, ref wDist);
            GameObject rightTile = tileScript.grid.GetNeighboringTile(tileScript, isPlayer, passThrough, tempNode.TileRange, Direction.EAST, boardingGrids, ref eDist);
            
            tileScript.highlighted = true;
        
            // Add neighboring tiles to queue if in range
            if (tempNode.TileRange > 0)
            {
                if (ValidHighlightTile(upTile, isPlayer, passThrough))
                {
                    tempNode.Up = new PathTreeNode(tempNode, upTile, tempNode.TileRange-nDist);
                    queue.Enqueue(tempNode.Up);
                }

                if (ValidHighlightTile(downTile, isPlayer, passThrough))
                {
                    tempNode.Down = new PathTreeNode(tempNode, downTile, tempNode.TileRange-sDist);
                    queue.Enqueue(tempNode.Down);
                }

                if (ValidHighlightTile(leftTile, isPlayer, passThrough))
                {
                    tempNode.Left = new PathTreeNode(tempNode, leftTile, tempNode.TileRange-wDist);
                    queue.Enqueue(tempNode.Left);
                }

                if (ValidHighlightTile(rightTile, isPlayer, passThrough))
                {
                    tempNode.Right = new PathTreeNode(tempNode, rightTile, tempNode.TileRange-eDist);
                    queue.Enqueue(tempNode.Right);
                }
            }
        }

        return root;
    }

    // Get neighboring tile with consideration for boarding grids
    private GameObject GetNeighboringTile(Tile startTile, bool isPlayer, bool passThrough, int range, Direction direction, List<Grid> boardingGrids, ref int distance)
    {
        if (startTile.highlighted) return null;
        GameObject destTile = GetTileInDirection(startTile.position, direction);
        if (destTile != null) return destTile; // Inside bounds of current grid
        float minDistSqr = float.MaxValue;
        // At an edge, search for valid boarding tiles
        foreach(Grid grid in boardingGrids)
        {
            if (startTile.grid.Equals(grid)) continue; // Must be a different grid
            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    if ((x > 0 && x < grid.width - 1) && (y > 0 && y < grid.height - 1)) continue; // Not at edge
                    GameObject tile = grid.GetTileAtPos(new Vector2Int(x, y));
                    if (!ValidHighlightTile(tile, isPlayer, passThrough)) continue; // Unit must be able to move
                    float distSqr = DataUtil.DistanceSqr(startTile.transform.position, tile.transform.position);
                    if (distSqr < minDistSqr && distSqr <= range * range) // Use the closest valid tile
                    {
                        minDistSqr = distSqr;
                        destTile = tile;
                    }
                }
            }
        }
        if(destTile != null) distance = Mathf.FloorToInt(Mathf.Sqrt(minDistSqr));
        return destTile;
    }

    // --------------------------------------------------------------
    // @desc: Tests whether a tile can be highlighted
    // @arg: tileToCheck - the tile to check
    // @arg: isPlayer    - whether the character is on the player crew
    // @arg: passThrough - makes every tile valid (but still can't be null)
    // @ret: bool        - whether the tile can be highlighted
    // --------------------------------------------------------------
    public static bool ValidHighlightTile(GameObject tileToCheck, bool isPlayer, bool passThrough = false)
    {
        bool valid = false;

        // null check
        if (tileToCheck != null)
        {
            // Get data from tile
            var tileScript = tileToCheck.GetComponent<Tile>();

            // Check if already highlighted
            if (tileScript.highlighted == false && tileScript.passable)
            {
                // Conditions to make a tile valid
                if (passThrough == false && tileScript.hasCharacter)
                {
                    if (tileScript.characterOn.GetComponent<Character>().IsPlayer() == isPlayer)
                    {
                        valid = true;
                    }
                }
                else
                {
                    valid = true;
                }
            }
        }

        return valid;
    }

    // Unhighlights all highlighted tiles in the grid
    public void ResetAllHighlights()
    {
        GameObject currentTile;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Get data from tile at current pos
                currentTile = GetTileAtPos(new Vector2Int(x,y));
                var currentTileScript = currentTile.GetComponent<Tile>();

                // Check if the tile is highlighted
                if (currentTileScript.highlighted)
                {
                    // Reset highlighting
                    currentTile.GetComponent<Renderer>().material = unselected;
                    currentTileScript.highlighted = false;
                }
            }
        }
    }

    public List<GameObject> GetAllTiles() {
        var list = new List<GameObject>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                list.Add(Tiles[x,y]);
            }
        }
        return list;
    }

    // Get a tile game object from a grid position
    public GameObject GetTileAtPos(Vector2Int pos)
    {
        GameObject tileAtPos = null;

        if (TilePosInRange(pos))
        {
            tileAtPos = Tiles[pos.x, pos.y];
        }

        return tileAtPos;
    }

    public GameObject GetTileInDirection(Vector2Int pos, Direction direction)
    {
        return GetTileAtPos(new Vector2Int(pos.x + direction.xMove, pos.y + direction.yMove));
    }

    public GameObject GetTileNorth(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x, pos.y + 1));
    }

    public GameObject GetTileSouth(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x, pos.y - 1));
    }

    public GameObject GetTileEast(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x + 1, pos.y));
    }

    public GameObject GetTileWest(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x - 1, pos.y));
    }

    // Get a character game object from a grid position (if there is one)
    public GameObject GetCharacterAtPos(Vector2Int pos)
    {
        GameObject charaterAtPos = null;
        GameObject tileAtPos = GetTileAtPos(pos);

        if (tileAtPos != null)
        {
            // Get character from tile
            var tileScript = tileAtPos.GetComponent<Tile>();
            charaterAtPos = tileScript.characterOn;
        }

        return charaterAtPos;
    }

    // Return list of all interactable grid objects from the active unit position
    public List<GameObject> GetInteractableTileObjects(Vector2Int pos)
    {
        List<GameObject> tileObjects = new List<GameObject>();
        Tile tile = GetTileAtPos(pos).GetComponent<Tile>();
        if(tile.hasObject && !tile.hasGridLink && tile.objectOn != null) tileObjects.Add(tile.objectOn);
        if(GetTileNorth(pos) != null)
        {
            tile = GetTileNorth(pos).GetComponent<Tile>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        if(GetTileSouth(pos) != null)
        {
            tile = GetTileSouth(pos).GetComponent<Tile>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        if(GetTileWest(pos) != null)
        {
            tile = GetTileWest(pos).GetComponent<Tile>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        if(GetTileEast(pos) != null)
        {
            tile = GetTileEast(pos).GetComponent<Tile>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        return tileObjects;
    }

    public void MoveUnitToGrid(Tile unitTile)
    {
        // ----- Move the active character to the other grid -----
        Debug.Log("Moving character to another grid");

        // Add character to target grid
        unitTile.targetGrid.SpawnCharacter(unitTile.characterOn, unitTile.targetTile.position, false);

        // Camera culling
        unitTile.characterOn.layer = unitTile.targetGrid.gridLayer;
        cam.gameObject.GetComponent<CameraController>().SetLayerMode((CameraController.Layermode)unitTile.characterOn.layer);

        // Remove character from current grid
        unitTile.characterOn = null;
        unitTile.hasCharacter = false;
    }
}
