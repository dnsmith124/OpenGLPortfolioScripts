using System;

[Serializable]
public class DialogueOption
{
    public string Text;
    public int NextNodeId; // ID of the node that this option leads to
}