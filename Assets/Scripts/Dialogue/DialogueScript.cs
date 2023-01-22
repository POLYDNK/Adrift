using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//the dialogue box will print out the text at a set speed
//click anywhere to move to the next line
//once done the box will close

public class DialogueScript : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            if(textComponent.text == lines[index]) 
            {
                NextLine();
            }
            
            else 
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if(index < lines.Length - 1) {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }

        else {
            //gameObject.setActive(false);
            //supposed to end the scene but is giving an error... helpppp
        }
    }

    IEnumerator TypeLine()
    {
        foreach(char c in lines[index].ToCharArray()) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
