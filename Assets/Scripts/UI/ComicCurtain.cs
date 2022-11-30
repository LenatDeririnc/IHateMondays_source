using System;
using DG.Tweening;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace SceneManager
{
    public class ComicCurtain : LoadingCurtainBase
    {
        public ComicStripAnimator animator;
        public ComicStripFadeAnimator fadeAnimator;

        public AudioClip musicIntro;
        public AudioClip musicLoop;
        public AudioSource whooshSource;

        private bool _canLoadScene;

        public override bool CanActivateScene => _canLoadScene && animator.isAllPagesShown;

        public override bool CanLoadScene => _canLoadScene;

        public override void Hide(float speed, Action entireEndLoadAction = null)
        {
            _canLoadScene = false;
            fadeAnimator.Hide();
            animator.Hide();
        }

        public override void Show(float speed, Action entireEndLoadAction = null)
        {
            whooshSource.Play();
            
            _canLoadScene = false;
            fadeAnimator.Show();
            DOTween.Sequence()
                .InsertCallback(fadeAnimator.duration, () => _canLoadScene = true)
                .AppendInterval(0.05f)
                .AppendCallback(() => animator.Show())
                .SetUpdate(true);

            ServiceLocator.Get<InputBridgeService>()
                .SetCursorLocked(false);
            ServiceLocator.Get<AudioService>()
                .PlayBackgroundMusic(musicIntro, musicLoop, null, 1f);
        }
    }
}