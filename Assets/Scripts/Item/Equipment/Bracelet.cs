using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Bracelet : EquippableItem
{
    [FormerlySerializedAs("HealModifier")] [SerializeField]
    public int healModifier;
    

}
