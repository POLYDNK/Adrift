using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewIcons : MonoBehaviour
{
    // Public vars
    public BattleEngine battleScript; // Battle engine ref
    public GameObject icon; // Icon game object to attach to canvas
    public List<GameObject> goodIcons = new List<GameObject>(); // Good char icons list
    public List<GameObject> badIcons = new List<GameObject>(); // Bad char icons list
    public float centerSpacing; // Spacing away from center of the canvas
    public float iconSpacing; // Spacing for individual icons
    public float iconHeight; // Distance away from the bottom of the canvas
    
    // Private vars
    private int numOfCharacters = 0;
    private int goodChars = 0;
    private int badChars = 0;

    // Late update is called after Update()
    void LateUpdate()
    {
        // Get char count from battle engine
        int currNumOfChars = battleScript.units.Count;

        // Update when the # of chars in the battle system change
        if (numOfCharacters != currNumOfChars)
        {
            Debug.Log("Updating crew icons...");

            // For every character
            foreach (GameObject unit in battleScript.units)
            {
                bool isNewIcon = true;

                // Check both lists to see whether we have this icon already
                foreach (GameObject icon in goodIcons)
                {
                    if (unit == icon.GetComponent<UI_CharIcon>().myChar)
                    {
                        isNewIcon = false;
                    }
                }
                foreach (GameObject icon in badIcons)
                {
                    if (unit == icon.GetComponent<UI_CharIcon>().myChar)
                    {
                        isNewIcon = false;
                    }
                }

                // If this is a new icon-
                if (isNewIcon == true)
                {
                    // Spawn Icon
                    GameObject newIcon = Instantiate(icon, this.transform);
                    //Vector3 pos = newIcon.transform.position; // store pos

                    // Set Icon Data
                    UI_CharIcon iconScript = newIcon.GetComponent<UI_CharIcon>();
                    iconScript.myChar = unit;
                    newIcon.transform.GetChild(0).GetComponent<Image>().sprite = iconScript.myChar.GetComponent<CharacterStats>().icon;

                    // Position based on whether ally or enemy
                    if (iconScript.myChar.GetComponent<CharacterStats>().isPlayer())
                    {
                        // Add icon to list
                        goodIcons.Add(newIcon);

                        // Modify position and healthbar, set background
                        newIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-shiftAmount(goodChars, true), iconHeight);
                        newIcon.transform.GetChild(2).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(true);
                        newIcon.GetComponent<Image>().sprite = iconScript.backgroundAlly;

                        // Increment counter
                        goodChars++;
                    }
                    else
                    {
                        // Add icon to list
                        badIcons.Add(newIcon);

                        // Modify position and healthbar, set background
                        newIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(shiftAmount(badChars, false), iconHeight);
                        newIcon.transform.GetChild(2).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(false);
                        newIcon.GetComponent<Image>().sprite = iconScript.backgroundEnemy;

                        // Increment counter
                        badChars++;
                    }
                }
            }

            // Update number of characters
            numOfCharacters = currNumOfChars;
        }
    }

    private float shiftAmount(int n, bool goodIcon)
    {
        // Calculate shift
        float shift = centerSpacing + (n * iconSpacing);
        float halfW = transform.parent.transform.GetComponent<RectTransform>().rect.width / 2f - 50f;

        // If the calculated shift position is off the screen, then we'll have to adjust
        // the position of every character icon on that side of the screen
        if (shift > halfW)
        {
            float iconSpaceNew = (halfW - centerSpacing) / n;
            float newShift = shift;
            
            if (goodIcon)
            {
                for (int i = 0; i < goodIcons.Count; i++)
                {
                    GameObject currIcon = goodIcons[i];
                    newShift = centerSpacing + (i * iconSpaceNew);
                    currIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-newShift, iconHeight);
                }
            }
            else
            {
                for (int i = 0; i < badIcons.Count; i++)
                {
                    GameObject currIcon = badIcons[i];
                    newShift = centerSpacing + (i * iconSpaceNew);
                    currIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(newShift, iconHeight);
                }
            }

            shift = newShift; // for return
        }

        // Return calculated shift
        return shift;
    }
}
