using System;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

namespace Scenes
{
    public class Room : MonoBehaviour
    {
        private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        
        [SerializeField] private int _id;
        [SerializeField] private Transform parentMesh;

        public int ID => _id;
        private RoomsService _service;
        private MeshRenderer[] _renderers;
        private MaterialPropertyBlock _block;

        public bool isActive { get; private set; } = false;

        public void Construct(RoomsService service, bool isActive)
        {
            _service = service;
            _renderers = parentMesh.GetComponentsInChildren<MeshRenderer>();

            _block = new MaterialPropertyBlock();

            this.isActive = isActive;
            if (!isActive)
                Hide();
        }

        public void OnEnterRoom()
        {
            if (isActive)
                return;
            
            EnterRoom();
            isActive = true;
        }

        public void OnExitRoom()
        {
            if (!isActive)
                return;
            
            ExitRoom();
            isActive = false;
        }

        private void Hide()
        {
            _block.SetFloat(Cutoff, 1);
            UpdateMaterials();
        }

        private void EnterRoom()
        {
            DOTween.To(
                () => _block.GetFloat(Cutoff), 
                value => { 
                    _block.SetFloat(Cutoff, value);
                    UpdateMaterials();
                }, 
                0,
                _service.RoomEnterDuration)
                .SetEase(_service.RoomEnterEase);
        }

        private void ExitRoom()
        {
            DOTween.To(
                () => _block.GetFloat(Cutoff), 
                value => { 
                    _block.SetFloat(Cutoff, value); 
                    UpdateMaterials(); }, 
                1,
                _service.RoomEnterDuration)
                .SetEase(_service.RoomExitEase);
        }

        private void UpdateMaterials()
        {
            foreach (var r in _renderers) {
                r.SetPropertyBlock(_block);
            }
        }
    }
}