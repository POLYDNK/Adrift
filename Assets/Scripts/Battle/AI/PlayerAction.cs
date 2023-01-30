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
    private Character character;

    //The target of the character's actions (might be empty) ### MIGHT NEED TO MAKE THIS A LIST ###
    private Character target;

    //The ability the character used on its turn (might be empty)
    private Ability action;

    //Whether or not the character moved this turn
    private bool movement;
    
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
        character = null;
        target = null;
        action = null;
        movement = false;
    }

    //Parameterized Constructor
    public PlayerAction(Character character, Character target, Ability action, bool movement)
    {
        this.character = character;
        this.target = target;
        this.action = action;
        this.movement = movement;
    }

    //Temporary test constructor while waiting for EnemyStats to be implemented
    public PlayerAction(Character character, Ability action, bool movement)
    {
        this.character = character;
        this.action = action;
        this.movement = movement;
    }

    //Destructor
    ~PlayerAction()
    {
        //Not likely needed but always good to have for debugging / just in case
    }

    public Character GetCharacter()
    {
        return character;
    }

    public Character GetTarget()
    {
        return target;
    }

    public Ability GetAbility()
    {
        return action;
    }

    public bool GetMovement()
    {
        return movement;
    }


}
