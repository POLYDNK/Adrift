using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BarteringTrigger : MonoBehaviour
{
    [FormerlySerializedAs("inkJSON")]
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJson;

    private void Update()
    {
        if (!BarteringManager.GetInstance().DialogueIsPlaying)
        {
            if (InputManager.GetInstance().GetInteractPressed())
            {
                BarteringManager.GetInstance().EnterDialogueMode(inkJson);
            }
        }
    }
}