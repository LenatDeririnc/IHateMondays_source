using System;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace SceneManager
{
    public class ComicCurtain : LoadingCurtainBase
    {
        public ComicStripAnimator animator;

        public AudioClip musicIntro;
        public AudioClip musicLoop;
        
        public override bool CanActivateScene => animator.isAllPagesShown;
        
        public override void Hide(float speed, Action entireEndLoadAction = null)
        {
            animator.Hide();
        }

        public override void Show(float speed, Action entireEndLoadAction = null)
        {
            animator.Show();
            
            ServiceLocator.Get<InputBridgeService>()
                .SetCursorLocked(false);
            ServiceLocator.Get<AudioService>()
                .PlayBackgroundMusic(musicIntro, musicLoop, 1f);
        }
    }
}