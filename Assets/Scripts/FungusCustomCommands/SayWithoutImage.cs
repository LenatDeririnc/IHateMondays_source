using System;
using Fungus;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace FungusCustomCommands
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Custom", 
                 "Say Without Image", 
                 "Writes text in a dialog box.")]
    [AddComponentMenu("")]
    public class SayWithoutImage : Say
    {
        public override void OnEnter()
        {
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            // Override the active say dialog if needed
            if (character != null && character.SetSayDialog != null)
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                Continue();
                return;
            }
    
            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);
            sayDialog.SetCharacter(character);

            string displayText = storyText;

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }

            string subbedText = flowchart.SubstituteVariables(displayText);

            sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate {
                Continue();
            });
        }
    }
}