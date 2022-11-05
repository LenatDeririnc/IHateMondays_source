using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    public GameObject[] Buttons;
    [SerializeField] private GameController _gameController;

    private void OnEnable()
    {
        bool isActive = false;
        foreach (GameObject button in Buttons)
        {
            if (button.active)
            {
                isActive = true;
                break;
            }
        }

        if(!isActive)
        {
            _gameController.EndGame();
        }
    }
}
