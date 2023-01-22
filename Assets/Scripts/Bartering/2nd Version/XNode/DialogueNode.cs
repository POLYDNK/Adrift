using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNode : BaseNode {

	[Input] public int entry;				//For xnode this is the entry point of the node.
	[Output] public int exit;				//For xnode this is the exit point of the node.

	public string speakerName;				//Speakers name
	public string dialogueLine;				//The line of dialogue being said by the speaker
	public Sprite sprite;					//Sprite of the person speaking

	public override string GetString()
	{
		return "DialogueNode/" + speakerName + "/" + dialogueLine;
	}

	public override Sprite GetSprite()
	{
		return sprite;
	}
	
}