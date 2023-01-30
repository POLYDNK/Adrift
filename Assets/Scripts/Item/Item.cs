
using UnityEngine;
using UnityEngine.Serialization;


//[CreateAssetMenu] //allows creating of item assets directly unity by right clicking project
public class Item : ScriptableObject
{
  [FormerlySerializedAs("ItemName")] public string itemName; //item name
  [FormerlySerializedAs("Icon")] public Sprite icon; //item image
  [FormerlySerializedAs("Type")] public string type; //item type
  [FormerlySerializedAs("Stats")] public string stats; //item parameters
  [FormerlySerializedAs("Description")] public string description;

}
