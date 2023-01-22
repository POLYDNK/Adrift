/// @author: Bryson Squibb
/// @date: 10/08/2022
/// @description: this script spawns a grid of tile objects, then
/// it provides an interface to interact with said spawned grid

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    [SerializeField] public GridBehavior cabinGrid; // GridRef
    [SerializeField] public GameObject linkBox; // Used as a linking visual
    [SerializeField] public int gridNum; // Grid number 
    [SerializeField] public int tilesize; // Size of each tile
    [SerializeField] public int gridLayer; // Layer used for camera culling

    // Grid vars
    public GameObject Tile; // A cell or tile that makes up the grid
    public GameObject [,] grid; // The actual grid, represented by a 2d array
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
            crew.GetComponent<CrewSystem>().initCharacters();
            List<GameObject> characters = crew.GetComponent<CrewSystem>().characters;
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
        grid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position of the tile to create
                Vector2Int xy = new Vector2Int(x, y);
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y + passableTiles.GetHeightOffsetAtPos(xy), transform.position.z + y*tilesize);
                
                // Create the tile
                GameObject newTile = Instantiate(Tile, pos, transform.rotation, this.transform);

                // Set the data of the newly created tile
                newTile.name = "Tile " + x + " " + y;
                var newTileScript = newTile.GetComponent<TileScript>();
                newTileScript.position.x = x;
                newTileScript.position.y = y;
                newTileScript.myLayer = gridLayer;

                // Check the grid map for impassible tiles
                if (passableTiles.GetFlagAtPos(xy))
                {
                    SetTileToImpassible(newTile);
                }

                // Link the new tile to a position on the grid
                grid[x, y] = newTile;
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
                spawningTile = grid[randX, randY];

                // Get data from tile object
                var tilesScript = spawningTile.GetComponent<TileScript>();

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
                    tilesScript.characterOn.GetComponent<CharacterStats>().gridPosition = new Vector2Int(randX, randY);
                    tilesScript.characterOn.GetComponent<CharacterStats>().myGrid = this.gameObject;
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
        spawningTile = grid[spawnPos.x, spawnPos.y];

        // Get data from tile object
        var tilesScript = spawningTile.GetComponent<TileScript>();

        // Check whether the tile already has a character on it
        // + whether the tile is a passable tile
        if (tilesScript.hasCharacter == false && tilesScript.passable)
        {
            // Spawn the character if valid
            Vector3 tilePos = spawningTile.transform.position;
            Vector3 pos = new Vector3(tilePos.x, tilePos.y+0.5f, tilePos.z);
            Debug.Log("Spawning character as position " + spawnPos.x + " " + spawnPos.y);
            character.GetComponent<CharacterStats>().gridPosition = spawnPos;
            character.GetComponent<CharacterStats>().myGrid = this.gameObject;
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
        tile = grid[gridPos.x, gridPos.y];
        Debug.Log("Attaching object to position " + gridPos.x + " " + gridPos.y);

        // Get data from tile object
        var tilesScript = tile.GetComponent<TileScript>();
    
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

    public bool RemoveCharacter(GameObject character) {
        Vector2Int tilePos = character.GetComponent<CharacterStats>().gridPosition;
        var tile = GetTileAtPos(tilePos).GetComponent<TileScript>();
        if(tile.characterOn == character) {
            character.GetComponent<CharacterStats>().removeFromGrid();
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
            GameObject sourceTile = grid[sourcePos.x, sourcePos.y];
            var sourceTileScript = sourceTile.GetComponent<TileScript>();

            // Get tile on dest position
            GameObject destTile = grid[destPos.x, destPos.y];
            var destTileScript = destTile.GetComponent<TileScript>();

            Vector2Int path = new Vector2Int(0, 0);
            if(!sourceTileScript.hasCharacter) return false;
            int xDist = destPos.x - sourcePos.x;
            int yDist = destPos.y - sourcePos.y;
            if(xDist > 0) {
                for(int i = 1; i <= xDist; i++) {
                    var tileScript = grid[sourcePos.x + i, sourcePos.y].GetComponent<TileScript>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(i, 0);
                }
            }
            else {
                for(int i = -1; i >= xDist; i--) {
                    var tileScript = grid[sourcePos.x + i, sourcePos.y].GetComponent<TileScript>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(i, 0);
                }
            }
            if(yDist > 0) {
                for(int i = 1; i <= yDist; i++) {
                    var tileScript = grid[sourcePos.x, sourcePos.y + i].GetComponent<TileScript>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(0, i);
                }
            }
            else {
                for(int i = -1; i >= yDist; i--) {
                    var tileScript = grid[sourcePos.x, sourcePos.y + i].GetComponent<TileScript>();
                    if(tileScript.hasCharacter || !tileScript.passable) break;
                    else path = new Vector2Int(0, i);
                }
            }

            if(path.x != 0 || path.y != 0) {
                charToMove = sourceTileScript.characterOn;
                var pos = new Vector2Int(sourcePos.x + path.x, sourcePos.y + path.y);
                var tileScript = GetTileAtPos(pos).GetComponent<TileScript>();

                charToMove.GetComponent<FollowPath>().abilityDist = path;

                // Move camera to destPos
                cam.GetComponent<CameraControl>().SetCameraFollow(charToMove);

                // Set source tile data
                sourceTileScript.hasCharacter = false;
                sourceTileScript.characterOn = null;

                // Set destination tile data
                tileScript.hasCharacter = true;
                tileScript.characterOn = charToMove;

                charToMove.GetComponent<CharacterStats>().gridPosition = pos;

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
    // @arg: sourcePos - logical grid position with a character on it
    // @arg: destPos   - logical grid position to move the character to
    // @ret: bool      - whether the move is successful or not
    // --------------------------------------------------------------
    public bool PathCharacterOnTile(Vector2Int sourcePos, Vector2Int destPos, bool onlyHighlighted)
    {
        GameObject charToMove = null;
        bool moveSuccess = false;

        // Check whether tiles are in range
        if (TilePosInRange(sourcePos) && TilePosInRange(destPos))
        {
            // Get tile on source position
            GameObject sourceTile = grid[sourcePos.x, sourcePos.y];
            var sourceTileScipt = sourceTile.GetComponent<TileScript>();

            // Get tile on dest position
            GameObject destTile = grid[destPos.x, destPos.y];
            var destTileScript = destTile.GetComponent<TileScript>();

            // Get character on source tile
            if (sourceTileScipt.hasCharacter && !destTileScript.hasCharacter && destTileScript.passable)
            {
                // Only move to highlighted tiles
                if (!onlyHighlighted || destTileScript.highlighted)
                {
                    Debug.Log("Moving character to tile " + destPos.x + " " + destPos.y);
                    charToMove = sourceTileScipt.characterOn;

                    // Move character to destPos
                    destTileScript.pathRef.PathToRootOnStack(charToMove.GetComponent<FollowPath>().pathToFollow);

                    // Move camera to destPos
                    cam.GetComponent<CameraControl>().SetCameraFollow(charToMove);

                    // Set source tile data
                    sourceTileScipt.hasCharacter = false;
                    sourceTileScipt.characterOn = null;

                    // Set destination tile data
                    destTileScript.hasCharacter = true;
                    destTileScript.characterOn = charToMove;

                    charToMove.GetComponent<CharacterStats>().gridPosition = destPos;

                    moveSuccess = true;
                }
            }
            else
            {
                Debug.Log("PathCharacterOnTile: Error! source tile does not have a character");
            }
        }
        else
        {
            Debug.Log("PathCharacterOnTile: Error! tile source or dest position is out of range");
        }
        
        return moveSuccess;
    }

    // --------------------------------------------------------------
    // @desc: Move a character along an automatically generated path
    // @arg: sourcePos - logical grid position with a character on it
    // @arg: destPos   - logical grid position to move the character to
    // @arg: maxRange  - the maximum range the character can move
    // @ret: bool      - whether the move is successful or not
    // --------------------------------------------------------------
    public bool MoveTowardsTile(Vector2Int sourcePos, Vector2Int destPos, bool onlyHighlighted, int maxRange)
    {
        GameObject charToMove = null;
        bool moveSuccess = false;


        // Check whether tiles are in range
        if (TilePosInRange(sourcePos) && TilePosInRange(destPos))
        {
            // Get tile on source position
            GameObject sourceTile = grid[sourcePos.x, sourcePos.y];
            var sourceTileScipt = sourceTile.GetComponent<TileScript>();

            // Get tile on dest position
            GameObject destTile = grid[destPos.x, destPos.y];
            var destTileScript = destTile.GetComponent<TileScript>();

            // Make a new path tree
            //sourceTileScipt.pathRef = GetAllPathsFromTile(sourceTile, maxRange+1, true);

            // Get character on source tile
            if (sourceTileScipt.hasCharacter && !destTileScript.hasCharacter && destTileScript.passable)
            {
                // Only move to highlighted tiles
                if (!onlyHighlighted || destTileScript.highlighted)
                {
                    Debug.Log("Moving character to tile " + destPos.x + " " + destPos.y);
                    charToMove = sourceTileScipt.characterOn;

                    // Get a list of tiles towards the destination
                    List<PathTreeNode> PathTowardsDest = destTileScript.pathRef.PathToRootList();

                    // Cut down stack to max range
                    while (PathTowardsDest.Count-1 > maxRange)
                    {
                        PathTowardsDest.RemoveAt(0);
                    }

                    // Destination tile is the first element in the list
                    destTile = PathTowardsDest[0].myTile;

                    // Get the data from the new destination tile
                    destTileScript = destTile.GetComponent<TileScript>();

                    // Move character to the new destination, instead
                    destTileScript.pathRef.PathToRootOnStack(charToMove.GetComponent<FollowPath>().pathToFollow);

                    // Move camera to destPos
                    cam.GetComponent<CameraControl>().SetCameraFollow(charToMove);

                    // Set source tile data
                    sourceTileScipt.hasCharacter = false;
                    sourceTileScipt.characterOn = null;

                    // Set destination tile data
                    destTileScript.hasCharacter = true;
                    destTileScript.characterOn = charToMove;

                    Debug.Log("gridPosition update");
                    charToMove.GetComponent<CharacterStats>().gridPosition = destTileScript.position;

                    moveSuccess = true;
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
        var tileScript = tile.GetComponent<TileScript>();

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
    public bool CreateLinkTile(Vector2Int sourceTilePos, Vector2Int destTilePos, GridBehavior targetGridScript)
    {
        bool linkSuccess = false;

        // Get tiles from positions
        GameObject sourceTile = GetTileAtPos(sourceTilePos);
        GameObject destTile   = targetGridScript.GetTileAtPos(destTilePos);

        // Check get
        if (sourceTile != null && destTile != null)
        {
            // Get tile scripts
            var sourceScript = sourceTile.GetComponent<TileScript>();
            var destScript = destTile.GetComponent<TileScript>();

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

    // --------------------------------------------------------------
    // @desc: Creates a tree data structure based on what moves a 
    // character can take from a given tile position
    // @arg: pos          - the root tile position
    // @arg: range        - how many tiles the character can move
    // @arg: passThrough  - makes every tile valid
    // @ret: PathTreeNode - the constructed tree
    // --------------------------------------------------------------
    public PathTreeNode GetAllPathsFromTile(GameObject tile, int range, bool passThrough = false)
    {
        // Reset highlights beforehand
        ResetAllHighlights();

        var startTile = tile.GetComponent<TileScript>();
        // Create root node
        PathTreeNode root = new PathTreeNode();
        root.myTile = tile;
        root.tileRange = range;

        // Character should always be here, but just in case
        if(!startTile.hasCharacter) {
            Debug.Log("Error: cannot retrieve paths from tile since its character is null.");
            return root;
        }

        bool isPlayer = startTile.characterOn.GetComponent<CharacterStats>().isPlayer();

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
            var tileRend = tempNode.myTile.GetComponent<Renderer>();
            var tileScript = tempNode.myTile.GetComponent<TileScript>();
            Vector2Int tilePos = tileScript.position;

            // Highlight tile
            tileScript.highlighted = true;

            // Get neighboring tiles
            GameObject upTile = GetTileAtPos(new Vector2Int(tilePos.x, tilePos.y - 1));
            GameObject downTile = GetTileAtPos(new Vector2Int(tilePos.x, tilePos.y + 1));
            GameObject leftTile = GetTileAtPos(new Vector2Int(tilePos.x - 1, tilePos.y));
            GameObject rightTile = GetTileAtPos(new Vector2Int(tilePos.x + 1, tilePos.y));
        
            // Add neighboring tiles to queue if in range
            if (tempNode.tileRange > 0)
            {
                if (ValidHighlightTile(upTile, isPlayer, passThrough))
                {
                    tempNode.up = new PathTreeNode(tempNode, upTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.up);
                }

                if (ValidHighlightTile(downTile, isPlayer, passThrough))
                {
                    tempNode.down = new PathTreeNode(tempNode, downTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.down);
                }

                if (ValidHighlightTile(leftTile, isPlayer, passThrough))
                {
                    tempNode.left = new PathTreeNode(tempNode, leftTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.left);
                }

                if (ValidHighlightTile(rightTile, isPlayer, passThrough))
                {
                    tempNode.right = new PathTreeNode(tempNode, rightTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.right);
                }
            }
        }

        return root;
    }

    // --------------------------------------------------------------
    // @desc: Tests whether a tile can be highlighted
    // @arg: tileToCheck - the tile to check
    // @arg: isPlayer    - whether the character is on the player crew
    // @arg: passThrough - makes every tile valid (but still can't be null)
    // @ret: bool        - whether the tile can be highlighted
    // --------------------------------------------------------------
    private bool ValidHighlightTile(GameObject tileToCheck, bool isPlayer, bool passThrough = false)
    {
        bool valid = false;

        // null check
        if (tileToCheck != null)
        {
            // Get data from tile
            var tileScript = tileToCheck.GetComponent<TileScript>();

            // Check if already highlighted
            if (tileScript.highlighted == false && tileScript.passable)
            {
                // Conditions to make a tile valid
                if (passThrough == false && tileScript.hasCharacter)
                {
                    if (tileScript.characterOn.GetComponent<CharacterStats>().isPlayer() == isPlayer)
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
                var currentTileScript = currentTile.GetComponent<TileScript>();

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
                list.Add(grid[x,y]);
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
            tileAtPos = grid[pos.x, pos.y];
        }

        return tileAtPos;
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
            var tileScript = tileAtPos.GetComponent<TileScript>();
            charaterAtPos = tileScript.characterOn;
        }

        return charaterAtPos;
    }

    // Return list of all interactable grid objects from the active unit position
    public List<GameObject> GetInteractableTileObjects(Vector2Int pos)
    {
        List<GameObject> tileObjects = new List<GameObject>();
        TileScript tile = GetTileAtPos(pos).GetComponent<TileScript>();
        if(tile.hasObject && !tile.hasGridLink && tile.objectOn != null) tileObjects.Add(tile.objectOn);
        if(GetTileNorth(pos) != null)
        {
            tile = GetTileNorth(pos).GetComponent<TileScript>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        if(GetTileSouth(pos) != null)
        {
            tile = GetTileSouth(pos).GetComponent<TileScript>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        if(GetTileWest(pos) != null)
        {
            tile = GetTileWest(pos).GetComponent<TileScript>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        if(GetTileEast(pos) != null)
        {
            tile = GetTileEast(pos).GetComponent<TileScript>();
            if(tile != null && tile.hasObject && !tile.hasGridLink && tile.objectOn != null && !tile.objectOn.GetComponent<GridObject>().overlapCharacter) tileObjects.Add(tile.objectOn);
        }
        return tileObjects;
    }

    public void MoveUnitToGrid(GameObject unitTile)
    {
        TileScript unitTileScript = unitTile.GetComponent<TileScript>();
        // ----- Move the active character to the other grid -----
        Debug.Log("Moving character to another grid");

        // Add character to target grid
        unitTileScript.targetGrid.SpawnCharacter(unitTileScript.characterOn, unitTileScript.targetTile.position, false);

        // Camera culling
        unitTileScript.characterOn.layer = unitTileScript.targetGrid.gridLayer;
        cam.gameObject.GetComponent<CameraControl>().SetLayerMode((CameraControl.LAYERMODE)unitTileScript.characterOn.layer);

        // Remove character from current grid
        unitTileScript.characterOn = null;
        unitTileScript.hasCharacter = false;
    }
}
