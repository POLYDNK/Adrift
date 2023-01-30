/// @author: Jayden Wang
/// @date: 10/17/2022
/// @description: A variable sized list that holds player actions for calculating enemy behavior. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionList
{
    ///////////////////////
    // Private Variables //
    ///////////////////////

    private List<PlayerAction> playerActions;
    private int capacity;
    
    // Start is called before the first frame update
    void Start()
    {
        SortPlayerActionsByName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Constructor
    public PlayerActionList()
    {
        playerActions = new List<PlayerAction>();
        capacity = 50;
    }

    //Destructor
    ~PlayerActionList()
    {
        //Not likely needed but always good to have for debugging / just in case
    }

    /////////////
    // Methods //
    /////////////
    public PlayerAction this[int index]
    {
        get
        {
            PlayerAction temp = playerActions[index];
            return temp;
        }

        set
        {
            playerActions[index] = value;
        }
    }

    public void Add(PlayerAction newAction)
    {
        //For now, we're experimenting with size 50 queue for holding player actions. If we're below 50, just add. Otherwise, get rid of the oldest action and then add.
        if(playerActions.Count < capacity)
        {
            playerActions.Add(newAction);
        }
        else
        {
            playerActions.RemoveAt(0);
            playerActions.Add(newAction);
        }
    }

    // public void clear() 
    // {
    //     if(_playerActions.Count > 0)
    //     {
    //         _playerActions.RemoveAt(0);
    //     }
    // }

    // public PlayerAction Peek()
    // {
    //     return _playerActions.Peek();
    // }

    public bool IsEmpty()
    {
        return playerActions.Count == 0;
    }

    public int Count()
    {
        return playerActions.Count;
    }

    /// <summary>
    /// Sorts Player Actions by character name.
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> SortPlayerActionsByName()
    {
        PlayerAction[] list1  =  playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetCharacter().displayName.CompareTo(b.GetCharacter().displayName));

       

        return realList;
    }

    /// <summary>
    /// Sorts Player actions by enemy name
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> SortPlayerActionsByEnemyName()
    {
        PlayerAction[] list1 = playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetTarget().displayName.CompareTo(b.GetTarget().displayName));

        

        return realList;
    }

    /// <summary>
    /// Sorts player action by ability name
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> SortPlayerActionsByAbilityId()
    {
        PlayerAction[] list1 = playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetAbility().displayName.CompareTo(b.GetAbility().displayName));

        

        return realList;
    }

    /// <summary>
    /// Sorts player actions by movement, this is just a true false sort.
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> SortPlayerActionsByMovement()
    {
        PlayerAction[] list1 = playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetMovement().CompareTo(b.GetMovement()));



        return realList;
    }


    public bool SearchForCharacterStats(PlayerAction characterID)
    {
        bool exists = playerActions.Contains(characterID);
        return exists;
    }

    public bool SearchForTarget(PlayerAction targetID)
    {
        bool exists = playerActions.Contains(targetID);
        return exists;
    }

    public bool SearchForAction(PlayerAction actionID)
    {
        bool exists = playerActions.Contains(actionID);
        return exists;
    }

    public bool SearchForMovement(PlayerAction movementID)
    {
        bool exists = playerActions.Contains(movementID);
        return exists;
    }

}

