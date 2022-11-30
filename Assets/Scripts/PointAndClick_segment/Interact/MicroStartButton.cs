using System.Collections;
using System.Collections.Generic;
using Plugins.ServiceLocator;
using SceneManager;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;

public class MicroStartButton : InteractElement
{
    [SerializeField] private Open _microDoor;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private Animator _animator;
    [SerializeField] private SceneLink _sceneLink;
    [SerializeField] private SoundHub _soundHub;
    public override void Use()
    {
        _microDoor.isLocked = true;
        _restartButton.SetActive(false);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        _animator.SetBool("EndAnim", true); 
        yield return new WaitForSeconds(1.5f);
        _soundHub.PlaySound(10);
        yield return new WaitForSeconds(5);
        _soundHub.StopSound();
        var sceneService = ServiceLocator.Get<SceneLoadingService>();
        sceneService.LoadScene(_sceneLink, CurtainType.ComicZeldaToRunner);
        Debug.Log("Load next scene");
    }
}
