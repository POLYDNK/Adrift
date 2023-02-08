using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class CharacterIcon : MonoBehaviour
{
    [SerializeField] public BattleEngine battleScript;
    [SerializeField] public TMP_Text turnOrderText;
    [SerializeField] public Sprite backgroundAlly, backgroundEnemy;
    public Character myChar;
    public RectangleBar myHealthBar;
    [FormerlySerializedAs("UpdateChar")] public bool updateChar = false;

    // Late update is called after update
    void LateUpdate()
    {
        // On character update for this icon
        if (updateChar)
        {
            transform.GetChild(2).GetChild(0).GetComponent<RectangleBar>().gradient = Character.HealthBarGradient(BattleEngine.IsAllyUnit(myChar));
            updateChar = false;
        }

        // Update healthbar
        if (myChar != null)
        {
            myHealthBar.SetMaxHealth(myChar.GetMaxHp());
            myHealthBar.SetHealth(myChar.hp);
        }
    }

    public void SelectMyChar()
    {
        if (myChar != null)
        {
            // Get Tile
            Vector2Int tilePos = myChar.gridPosition;
            GameObject myTile = myChar.myGrid.GetComponent<Grid>().GetTileAtPos(tilePos);

            // Select Char
            battleScript.SelectCharacter(myTile.GetComponent<Tile>());
        }
    }

    public void SetTurnOrderText(string turnText)
    {
        turnOrderText.text = turnText;
    }
}
