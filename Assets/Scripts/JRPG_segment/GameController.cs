using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Collections;

public enum Turn { Player, Boss }

public class GameController : MonoBehaviour
{
    [field: ShowNonSerializedField] public bool IsEndGame { get; private set; }

    public Turn _currentTurn;

    [Header("References")]
    [field: SerializeField] public TMP_Text DialogueText;
    [field: SerializeField] public TMP_Text NameText;
    [field: SerializeField] public GameObject DialoguePanel;
    [field: SerializeField] public GameObject ActionPanel;
    [SerializeField] private DialogueObject _dialogueObject;
    [SerializeField] private DialogueObject[] _playerDialogue;
    [SerializeField] private DialogueObject[] _bossDialogue;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private HpContainer _playerHP;

    [Header("Settings")]
    [SerializeField] private int _endGameCounterCondition;

    private bool _endGame;
    private int _bossAnswer;

    public void ButtonAction(int actionNumber)
    {
        _playerDialogue[actionNumber].DialogController();
        _bossAnswer = actionNumber;
        PlayerSentence();
    }

    public void NextTurn()
    {
        _currentTurn = _currentTurn == Turn.Player ? Turn.Boss : Turn.Player;

        if (_currentTurn == Turn.Boss)
        {
            _bossDialogue[_bossAnswer].DialogController();
        }
        else if (_currentTurn == Turn.Player)
        {
            DialoguePanel.SetActive(false);
            ActionPanel.SetActive(true);
        }
    }

    public void PlayerSentence()
    {
        ActionPanel.SetActive(false);
        DialoguePanel.SetActive(true);
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        _currentTurn = Turn.Boss;
        DialoguePanel.SetActive(true);
        _dialogueObject.DialogController();
    }

    public void EndGame()//TODO - добавить метод вызывающий анимацию эндгейма
    {
        _endGame = true;
        IsEndGame = true;
        ActionPanel.SetActive(false);

        if (!_endGame) { return; }

        DialoguePanel.SetActive(true);
        _nextButton.SetActive(false);
        _bossDialogue[4].DialogController();
        Debug.Log("Стул прилетел в ебучку");
        _endGame = false;
    }

    private void Awake()
    {
        DialoguePanel.SetActive(false);
        ActionPanel.SetActive(false);
        StartCoroutine(StartGame());
    }
}
