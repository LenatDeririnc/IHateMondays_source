using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private GameController _gameController;

    public Queue<string> GameDialog;

    private void Start()
    {
        GameDialog = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _dialogueText.text = dialogue.Sentences[0];
        _name.text = dialogue.Name;
        GameDialog.Clear();

        foreach (string text in dialogue.Sentences)
        {
            GameDialog.Enqueue(text);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (GameDialog.Count == 0)
        {
            EndDialogue();
            return;
        }

        string text = GameDialog.Dequeue();
        _dialogueText.text = text;
    }

    private void EndDialogue()
    {
        _gameController.NextTurn();
    }
}
