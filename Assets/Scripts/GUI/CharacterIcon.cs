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
    public GameObject myChar;
    public Character charScript = null;
    public RectangleBar myHealthBar;
    [FormerlySerializedAs("UpdateChar")] public bool updateChar = false;

    // Late update is called after update
    void LateUpdate()
    {
        // On character update for this icon
        if (updateChar)
        {
            charScript = myChar.GetComponent<Character>();
            transform.GetChild(2).GetChild(0).GetComponent<RectangleBar>().gradient = Character.HealthBarGradient(BattleEngine.IsAllyUnit(myChar));
            updateChar = false;
        }

        // Update healthbar
        if (charScript != null)
        {
            myHealthBar.SetMaxHealth(charScript.GetMaxHp());
            myHealthBar.SetHealth(charScript.hp);
        }
    }

    public void SelectMyChar()
    {
        if (myChar != null)
        {
            // Get Tile
            Vector2Int tilePos = charScript.gridPosition;
            GameObject myTile = charScript.myGrid.GetComponent<Grid>().GetTileAtPos(tilePos);

            // Select Char
            battleScript.SelectCharacter(myTile);
        }
    }

    public void SetTurnOrderText(string turnText)
    {
        turnOrderText.text = turnText;
    }
}
