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

    private List<PlayerAction> _playerActions;
    private int _capacity;
    
    // Start is called before the first frame update
    void Start()
    {
        sortPlayerActionsByName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Constructor
    public PlayerActionList()
    {
        _playerActions = new List<PlayerAction>();
        _capacity = 50;
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
            PlayerAction temp = _playerActions[index];
            return temp;
        }

        set
        {
            _playerActions[index] = value;
        }
    }

    public void add(PlayerAction newAction)
    {
        //For now, we're experimenting with size 50 queue for holding player actions. If we're below 50, just add. Otherwise, get rid of the oldest action and then add.
        if(_playerActions.Count < _capacity)
        {
            _playerActions.Add(newAction);
        }
        else
        {
            _playerActions.RemoveAt(0);
            _playerActions.Add(newAction);
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

    public bool isEmpty()
    {
        return _playerActions.Count == 0;
    }

    public int Count()
    {
        return _playerActions.Count;
    }

    /// <summary>
    /// Sorts Player Actions by character name.
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> sortPlayerActionsByName()
    {
        PlayerAction[] list1  =  _playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetCharacter().Name.CompareTo(b.GetCharacter().Name));

       

        return realList;
    }

    /// <summary>
    /// Sorts Player actions by enemy name
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> sortPlayerActionsByEnemyName()
    {
        PlayerAction[] list1 = _playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetTarget().Name.CompareTo(b.GetTarget().Name));

        

        return realList;
    }

    /// <summary>
    /// Sorts player action by ability name
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> sortPlayerActionsByAbilityId()
    {
        PlayerAction[] list1 = _playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetAbility().displayName.CompareTo(b.GetAbility().displayName));

        

        return realList;
    }

    /// <summary>
    /// Sorts player actions by movement, this is just a true false sort.
    /// </summary>
    /// <returns></returns>
    public List<PlayerAction> sortPlayerActionsByMovement()
    {
        PlayerAction[] list1 = _playerActions.ToArray();
        List<PlayerAction> realList = new List<PlayerAction>(list1);
        realList.Sort((a, b) => a.GetMovement().CompareTo(b.GetMovement()));



        return realList;
    }


    public bool searchForCharacterStats(PlayerAction characterID)
    {
        bool exists = _playerActions.Contains(characterID);
        return exists;
    }

    public bool searchForTarget(PlayerAction targetID)
    {
        bool exists = _playerActions.Contains(targetID);
        return exists;
    }

    public bool searchForAction(PlayerAction actionID)
    {
        bool exists = _playerActions.Contains(actionID);
        return exists;
    }

    public bool searchForMovement(PlayerAction movementID)
    {
        bool exists = _playerActions.Contains(movementID);
        return exists;
    }

}

