using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarteringTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Update()
    {
        if (!BarteringManager.GetInstance().dialogueIsPlaying)
        {
            if (InputManager.GetInstance().GetInteractPressed())
            {
                BarteringManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
    }
}