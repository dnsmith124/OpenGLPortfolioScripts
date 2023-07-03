using System;

[Serializable]
public class DialogueOption
{
    public string Text;
    // ID of the node that this option leads to
    public int NextNodeId;
    public string Condition;
}