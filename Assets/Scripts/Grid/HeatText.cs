/// @author: Bryson Squibb
/// @date: 01/31/2022
/// @description: this script controls
/// the text component used in showing 
/// the heat value of tile objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeatText : MonoBehaviour
{
    // Public vars
    [SerializeField] public TileScript myTile;
    [SerializeField] public TMP_Text floatingText;

    // Update is called once per frame
    void Update()
    {
        floatingText.text = myTile.heatVal.ToString();
    }
}
