using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private GameController _gameController;
    [SerializeField] private bool isUseTypeEffect;
    [SerializeField] private float _typeDuration;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private Turn _current;
    [SerializeField] HpContainer _playerHP;
    [SerializeField] HpContainer _bossHP;
    
    public Queue<string> GameDialog;

    private Coroutine _coroutine;
    private int _damage;

    private void Start()
    {
        GameDialog = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _dialogueText.text = dialogue.Sentences[0];
        _name.text = dialogue.Name;
        _damage = dialogue.DialogueDamage;
        _current = _gameController._currentTurn;
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

        if (isUseTypeEffect)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(TypeEffect(text));
        }
        else
        {
            _dialogueText.text = text;
        }
    }

    private IEnumerator TypeEffect(string text)
    {
        _dialogueText.text = "";

        foreach (char symbol in text.ToCharArray())
        {
            _dialogueText.text += symbol;
            yield return new WaitForSeconds(_typeDuration);
        }
    }

    private void EndDialogue()
    {
        if(_current == Turn.Player)
        {
            _bossHP.TakeDamage(_damage);
        }
        else if(_current == Turn.Boss)
        {
            _playerHP.TakeDamage(_damage);
        }

        _gameController.NextTurn();
    }
}
