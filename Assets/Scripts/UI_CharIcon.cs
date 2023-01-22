using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_CharIcon : MonoBehaviour
{
    [SerializeField] public BattleEngine battleScript;
    [SerializeField] public TMP_Text turnOrderText;
    [SerializeField] public Sprite backgroundAlly, backgroundEnemy;
    public GameObject myChar;
    public CharacterStats charScript = null;
    public HealthBar myHealthBar;
    public bool UpdateChar = false;

    // Late update is called after update
    void LateUpdate()
    {
        // On character update for this icon
        if (UpdateChar)
        {
            charScript = myChar.GetComponent<CharacterStats>();
            transform.GetChild(2).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(BattleEngine.isAllyUnit(myChar));
            UpdateChar = false;
        }

        // Update healthbar
        if (charScript != null)
        {
            myHealthBar.SetMaxHealth(charScript.getMaxHP());
            myHealthBar.SetHealth(charScript.HP);
        }
    }

    public void SelectMyChar()
    {
        if (myChar != null)
        {
            // Get Tile
            Vector2Int tilePos = charScript.gridPosition;
            GameObject myTile = charScript.myGrid.GetComponent<GridBehavior>().GetTileAtPos(tilePos);

            // Select Char
            battleScript.selectCharacter(myTile);
        }
    }

    public void SetTurnOrderText(string turnText)
    {
        turnOrderText.text = turnText;
    }
}
