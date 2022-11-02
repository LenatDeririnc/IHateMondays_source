using System;
using UnityEngine;

namespace MainMenu
{
    public class CanvasScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;

        public void SetActive(bool b)
        {
            _gameObject.SetActive(b);
        }
    }
}