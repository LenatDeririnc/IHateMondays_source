using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    public TextController TextController;

    public Dialogue Dialogue;

    public void DialogController()
    {
        TextController.StartDialogue(Dialogue);
    }
}
