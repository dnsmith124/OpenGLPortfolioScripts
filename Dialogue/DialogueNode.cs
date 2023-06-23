using System;
using System.Collections.Generic;

[Serializable]
public class DialogueNode
{
    public int Id; // unique ID of the node
    public string DialogueText; // text for this node
    public List<DialogueOption> Options; // options player can choose
    public bool Goodbye; // should the node end the current dialogue
}