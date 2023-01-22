using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewTurnOrder : MonoBehaviour
{
    // Public vars
    public BattleEngine battleScript; // Battle engine ref
    public GameObject icon; // Icon game object to attach to canvas
    public uint iconCount; // Number of icons to display from the turn order
    //public uint iconMaxLookahead; // Maximum number to look ahead in turn order
    public float iconSpacing; // Spacing for individual icons
    public float iconHeight; // Distance away from the bottom of the canvas
    public float leftMargin; // Spacing away from center of the canvas
    
    // Private vars
    private List<GameObject> icons = new List<GameObject>(); // Char icons list

    // Intitialize icons
    void Start()
    {
        for (uint i = 0; i < iconCount && i < 50; i++)
        {
            // Spawn Icon
            GameObject newIcon = Instantiate(icon, this.transform);

            // Add icon to list
            icons.Add(newIcon);

            // Modify rect transform
            RectTransform rect = icon.transform.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.0f, 0.0f);
            rect.anchorMax = new Vector2(0.0f, 0.0f);
            rect.anchoredPosition = new Vector2(shiftAmount(icons.Count), iconHeight);
        }
    }

    // Update Icons
    public void UpdateCrewTurnOrder()
    {
        // Populate all icons w/ characters
        for (int i = 0; i < icons.Count; i++)
        {
            GameObject currIcon = icons[i];
            var iconScript = currIcon.GetComponent<UI_CharIcon>();

            // Set icon data
            iconScript.battleScript = battleScript;
            iconScript.myChar = battleScript.turnQueue[i];
            currIcon.GetComponent<Image>().sprite = iconScript.myChar.GetComponent<CharacterStats>().isPlayer() ? iconScript.backgroundAlly : iconScript.backgroundEnemy;
            currIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconScript.myChar.GetComponent<CharacterStats>().icon;
            iconScript.UpdateChar = true;
            iconScript.turnOrderText.text = i.ToString();

            // Modify rect transform
            RectTransform rect = icon.transform.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.0f, 0.0f);
            rect.anchorMax = new Vector2(0.0f, 0.0f);
            rect.anchoredPosition = new Vector2(shiftAmount(i), iconHeight);
        }
    }

    private float shiftAmount(int n)
    {
        // Calculate shift
        float shift = leftMargin + (n * iconSpacing);
        float canvasWidth = transform.parent.transform.GetComponent<RectTransform>().rect.width - 50f;

        // If the calculated shift position is off the screen, then we'll have to adjust
        // the position of every character icon on that side of the screen
        if (shift > canvasWidth)
        {
            float iconSpaceNew = (canvasWidth - leftMargin) / n;
            float newShift = shift;

            for (int i = 0; i < icons.Count; i++)
            {
                GameObject currIcon = icons[i];
                newShift = leftMargin + (i * iconSpaceNew);
                currIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(newShift, iconHeight);
            }

            shift = newShift; // for return
        }

        // Return calculated shift
        return shift;
    }
}
