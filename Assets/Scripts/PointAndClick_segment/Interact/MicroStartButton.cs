using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroStartButton : InteractElement
{
    [SerializeField] private Open _microDoor;
    [SerializeField] private GameObject _restartButton;
    public override void Use()
    {
        _microDoor.isLocked = true;
        _restartButton.SetActive(false);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        Debug.Log("Animation maybe ???");
        yield return new WaitForSeconds(3);
        Debug.Log("BlackScreen");
        yield return new WaitForSeconds(2);
        Debug.Log("Load next scene");
    }
}
