using Plugins.ServiceLocator;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;
using UnityEngine.Playables;

public class SwitchToMainMenu : MonoBehaviour
{
    public PlayableDirector director;
    public SceneLink sceneLink;

    private void Awake()
    {
        director.stopped += _ =>
        {
            ServiceLocator.Get<SceneLoadingService>()
                .LoadScene(sceneLink);
        };
    }
}
