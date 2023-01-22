/// @author: Jayden Wang
/// @date: 10/17/2022
/// @description: A class object for easy resolution of player actions and their targets.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction
{
    ///////////////////////
    // Private Variables //
    ///////////////////////

    //The character whose turn is being recorded
    private CharacterStats _character;

    //The target of the character's actions (might be empty) ### MIGHT NEED TO MAKE THIS A LIST ###
    private CharacterStats _target;

    //The ability the character used on its turn (might be empty)
    private Ability _action;

    //Whether or not the character moved this turn
    private bool _movement;
    
    /////////////////////
    // Default Methods //
    /////////////////////

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /////////////
    // Methods //
    /////////////

    //Default Constructor
    public PlayerAction()
    {
        _character = null;
        _target = null;
        _action = null;
        _movement = false;
    }

    //Parameterized Constructor
    public PlayerAction(CharacterStats character, CharacterStats target, Ability action, bool movement)
    {
        _character = character;
        _target = target;
        _action = action;
        _movement = movement;
    }

    //Temporary test constructor while waiting for EnemyStats to be implemented
    public PlayerAction(CharacterStats character, Ability action, bool movement)
    {
        _character = character;
        _action = action;
        _movement = movement;
    }

    //Destructor
    ~PlayerAction()
    {
        //Not likely needed but always good to have for debugging / just in case
    }

    public CharacterStats GetCharacter()
    {
        return _character;
    }

    public CharacterStats GetTarget()
    {
        return _target;
    }

    public Ability GetAbility()
    {
        return _action;
    }

    public bool GetMovement()
    {
        return _movement;
    }


}
