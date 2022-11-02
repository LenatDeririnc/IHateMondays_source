using System.Collections;
using SceneManager.ScriptableObjects;
using UnityEngine;

namespace SceneManager.Tools
{
    public class SceneLoadForSeconds : MonoBehaviour
    {
        public SceneLoader Loader;
        public SceneLink loadScene;
        public float waitSeconds = 5f;
        void Start()
        {
            StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitSeconds);
            Loader.LoadScene(loadScene);
        }
    }
}
