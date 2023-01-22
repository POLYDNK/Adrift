using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifierController : MonoBehaviour
{
    // Character Ref
    public CharacterStats charStats;

    // Sprite Renderers
    public List<SpriteRenderer> statusIcons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update status icons
    void Update()
    {
        for (int i = 0; i < statusIcons.Count; i++)
        {
            if (i < charStats.statModifiers.Count)
            {
                Sprite statusSprite = charStats.statModifiers[i].statusIcon;

                if (statusSprite != null)
                {
                    statusIcons[i].sprite = statusSprite;
                }
                
                statusIcons[i].enabled = true;
            }
            else
            {
                statusIcons[i].enabled = false;
            }
        }
    }
}
