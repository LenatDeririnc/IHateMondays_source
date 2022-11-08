using System;
using UnityEngine;

namespace Fungus.Services
{
    public class FungusFactoryService : MonoBehaviour
    {
        [SerializeField] private int _currentDialogIndex;
        [SerializeField] private GameObject[] _sayDialogPrefabs;

        public SayDialog CreateDialogPrefab()
        {
            // Auto spawn a say dialog object from the prefab
            SayDialog sayDialog = null;
            
            GameObject prefab = _sayDialogPrefabs[_currentDialogIndex];
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab) as GameObject;
                go.SetActive(false);
                go.name = "SayDialog";
                sayDialog = go.GetComponent<SayDialog>();
            }
            else 
            {
                Debug.LogError("Not setted dialog prefabs!");
            }

            return sayDialog;
        }
    }
}