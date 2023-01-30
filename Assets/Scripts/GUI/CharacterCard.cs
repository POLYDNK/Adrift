using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class CharacterCard : MonoBehaviour
{
    // Public vars
    [SerializeField] public TMP_Text statsPanel;
    [SerializeField] public TMP_Text abilitiesPanel;
    [SerializeField] public Image portrait;
    [SerializeField] public Animator panelAnim;
    [FormerlySerializedAs("CurrentCharacter")] public Character currentCharacter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Update Stats Panel //
        if (CurrentCharacter != null)
        {
            statsPanel.text = statsList(CurrentCharacter);
        }
        */
    }

    public void Open(Character stats)
    {
        // Set character
        currentCharacter = stats;

        // Set portrait
        portrait.sprite = stats.icon;

        // Update Panels
        statsPanel.text = StatsList(currentCharacter);
        abilitiesPanel.text = AbilitiesList(currentCharacter);

        // Open panel
        panelAnim.Play("PanelOpen");
    }

    public void Open()
    {
        // Open panel
        panelAnim.Play("PanelOpen");
    }

    public void Close()
    {
        // Close panel
        panelAnim.Play("PanelClose");
    }

    public string StatsList(Character stats)
    {
        string text = "";

        text += "Max HP: " + stats.GetMaxHp() + "\n";
        text += "ATK: " + stats.atk + "\n";
        text += "STR: " + stats.GetStrength() + "\n";
        text += "DEF: " + stats.GetDefense() + "\n";
        text += "SPD: " + stats.GetSpeed() + "\n";
        text += "DEX: " + stats.GetDexterity() + "\n";
        text += "LCK: " + stats.GetLuck() + "\n";
        text += "AVO: " + stats.avo + "\n";
        text += "HIT: " + stats.hit + "\n";
        text += "MV: " + stats.GetMovement();

        return text;
    }

    public string AbilitiesList(Character stats)
    {
        string text = "";

        foreach(Ability ability in stats.abilities)
        {
            text += ability.displayName + "\n" + ability.description + "\n\n";
        }

        return text;
    }
}
