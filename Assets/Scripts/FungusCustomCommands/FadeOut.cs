using Fungus;
using Plugins.ServiceLocator;
using SceneManager;
using Services;
using UnityEngine;

namespace FungusCustomCommands
{
    [CommandInfo("Custom", "Fade Out", "Fade Out")]
    public class FadeOut : Command
    {
        public float fadeSpeed;

        public override void OnEnter()
        {
            var loadingCurtainManager = ServiceLocator.Get<SceneLoadingService>().LoadingCurtain;
            loadingCurtainManager.GetCurtain(CurtainType.AlphaTransition).Hide(fadeSpeed, Continue);
        }
        
        public override string GetSummary()
        {
            return $"Speed: {fadeSpeed}";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(255, 181, 33, 255);
        }
    }
}