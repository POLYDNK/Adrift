/// UNUSED SCRIPT

/// @author: Bryson Squibb
/// @date: 01/25/2023
/// @description: this script controls the behavior of an NPC.
/// It is meant to be attached to a character object.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    // Component References
    [SerializeField] public CharacterStats myCharacter;
    [SerializeField] public BattleEngine battleScript = null;
    [SerializeField] public FollowPath movePath;

    // Private Vars
    bool hasActed = false;

    public bool tryAttack()
    {
        bool hasAttacked = false;

        return hasAttacked;
    }
}