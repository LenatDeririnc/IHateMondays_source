using DG.Tweening;
using Services;
using Unity.VisualScripting;
using UnityEngine;

namespace Scenes
{
    public class Room : MonoBehaviour
    {
        private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        
        [SerializeField] private int _id;
        [SerializeField] private Transform parentMesh;

        public int ID => _id;
        private RoomsService _service;
        private MeshRenderer[] _renderers;
        private SpriteRenderer[] _spriteRenderers;
        private MaterialPropertyBlock _rendererBlock;
        private Color _spriteColor = Color.white;

        public bool isActive { get; private set; } = false;

        public void Construct(RoomsService service, bool isActive)
        {
            _service = service;
            _renderers = parentMesh.GetComponentsInChildren<MeshRenderer>();
            _spriteRenderers = parentMesh.GetComponentsInChildren<SpriteRenderer>();

            _rendererBlock = new MaterialPropertyBlock();

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
            _rendererBlock.SetFloat(Cutoff, 1);
            _spriteColor = Color.clear;
            UpdateMaterials();
        }

        private void EnterRoom()
        {
            DOTween.To(
                () => _rendererBlock.GetFloat(Cutoff), 
                value => { 
                    _rendererBlock.SetFloat(Cutoff, value);
                    _spriteColor = new Color(1, 1, 1, 1 - value);
                    UpdateMaterials();
                },
                0,
                _service.RoomEnterDuration)
                .SetEase(_service.RoomEnterEase);
        }

        private void ExitRoom()
        {
            DOTween.To(
                () => _rendererBlock.GetFloat(Cutoff), 
                value => { 
                    _rendererBlock.SetFloat(Cutoff, value);
                    _spriteColor = new Color(1,1,1, 1 - value);
                    UpdateMaterials();
                }, 
                1,
                _service.RoomEnterDuration)
                .SetEase(_service.RoomExitEase);
        }

        private void UpdateMaterials()
        {
            foreach (var r in _renderers) {
                r.SetPropertyBlock(_rendererBlock);
            }

            foreach (var r in _spriteRenderers) {
                r.color = _spriteColor;
            }
        }
    }
}