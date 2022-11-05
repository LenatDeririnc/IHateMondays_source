using NaughtyAttributes;

[System.Serializable]
public class Dialogue 
{
    public string Name;
    [ResizableTextArea]
    public string[] Sentences;
}
