using System;

namespace SceneManager
{
    public class ComicCurtain : LoadingCurtainBase
    {
        public ComicStripAnimator animator;
        
        public override bool CanActivateScene => animator.isAllPagesShown;
        
        public override void Hide(float speed, Action entireEndLoadAction = null)
        {
            animator.Hide();
        }

        public override void Show(float speed, Action entireEndLoadAction = null)
        {
            animator.Show();
        }
    }
}