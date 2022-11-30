using System;
using UnityEngine;

public class CameraForMenu : MonoBehaviour
{
    [SerializeField]
    private MenuCamera[] cameras;
    
    private void Update()
    {
        foreach (var c in cameras)
            c.cameraTransform.SetActive(c.menuTransform.activeSelf);
    }

    [Serializable]
    public struct MenuCamera
    {
        public GameObject menuTransform;
        public GameObject cameraTransform;
    }
}
