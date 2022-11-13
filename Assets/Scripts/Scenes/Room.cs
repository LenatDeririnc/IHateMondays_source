using System.Collections.Generic;
using DG.Tweening;
using Services;
using Unity.VisualScripting;
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
        private List<Material> _materials = new List<Material>();

        public bool isActive { get; private set; } = false;

        public void Construct(RoomsService service, bool isActive)
        {
            _service = service;
            var renderers = parentMesh.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer r in renderers) {
                foreach (Material mat in r.materials) {
                    if (!mat.HasFloat(Cutoff))
                        continue;
                    _materials.Add(mat);
                }
            }

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
            foreach (var m in _materials) {
                m.SetFloat(Cutoff, 1);
            }
        }

        private void EnterRoom()
        {
            foreach (var m in _materials) {
                DOTween.To(
                    () => m.GetFloat(Cutoff), 
                    value => { m.SetFloat(Cutoff, value); }, 
                    0,
                    _service.RoomEnterDuration).SetEase(_service.RoomEnterEase);
            }
        }

        private void ExitRoom()
        {
            foreach (var m in _materials) {
                DOTween.To(
                    () => m.GetFloat(Cutoff), 
                    value => { m.SetFloat(Cutoff, value); }, 
                    1,
                    _service.RoomEnterDuration).SetEase(_service.RoomExitEase);
            }
        }
    }
}