using NaughtyAttributes;

[System.Serializable]
public class Dialogue 
{
    public string Name;
    public int DialogueDamage;
    [ResizableTextArea]
    public string[] Sentences;
}
