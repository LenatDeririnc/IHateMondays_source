using System;
using System.Collections.Generic;
using Jrpg.Scripts.Timeline;
using Plugins.ServiceLocator;
using SceneManager;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;
using UnityEngine.Playables;

public class JrpgBossFightManager : MonoBehaviour, INotificationReceiver
{
    public UiJrpgStatusMessage uiStatusMessage;
    public UiJrpgDamageNumbersLayout uiDamageNumbers;
    public UiJrpgHealthPanel uiHealthPanel;

    public UiJrpgButtonPanel uiAction1Layer;
    public UiJrpgButtonPanel uiAction2Layer;

    public Transform bossTargetPoint;
    public Transform playerTargetPoint;

    public Animator animatorPlayer;

    public AudioSource sfxSource;
    public AudioClip clipMenuOpen;
    public AudioClip clipMenuClose;

    public int guyCurrentHp;
    public int guyMaxHp;

    public SceneLink nextScene;
    public CurtainType nextCurtain;

    public PlayableDirector startPlaybackDirector;
    public PlayableDirector bossAttackPlaybackDirector;
    public PlayableDirector eatSandwichDirector;
    public PlayableDirector attack1Director;
    public PlayableDirector attack2Director;
    public PlayableDirector attack3Director;
    public PlayableDirector attack4Director;
    
    private PlayableDirector _lastPlayedDirector;
    private PlayableOutput _lastSubscribedOutput;

    // Battle state
    private bool _hasSandwich = true;
    private bool _isAttack1Played;
    private bool _isAttack2Played;
    private bool _isAttack3Played;
    private bool _isAttack4Played;
    private bool _isFightFinished;

    private void Start()
    {
        PlayFightTimeline(startPlaybackDirector);
        uiHealthPanel.SetHp(guyMaxHp, guyCurrentHp, false);
    }

    public void PlayFightTimeline(PlayableDirector director)
    {
        if (_lastPlayedDirector)
        {
            _lastPlayedDirector.Stop();
            _lastPlayedDirector.stopped -= OnDirectorStopped;
        }

        if (_lastSubscribedOutput.IsOutputValid())
            _lastSubscribedOutput.RemoveNotificationReceiver(this);
        
        _lastPlayedDirector = director;

        if (!director) 
            return;
        
        _lastPlayedDirector.Play();
        
        _lastSubscribedOutput = _lastPlayedDirector.playableGraph.GetOutput(0);
        _lastSubscribedOutput.AddNotificationReceiver(this);

        director.stopped += OnDirectorStopped;
        
        HideBattleMenu();
    }

    private void ApplyDamageToPlayer()
    {
        animatorPlayer.SetInteger(
            "Health", 
            animatorPlayer.GetInteger("Health") - 1
        );
    }
    
    private void OnDirectorStopped(PlayableDirector director)
    {
        if (_isFightFinished)
            return;

        if (_isAttack1Played && _isAttack2Played && _isAttack3Played && _isAttack4Played)
        {
            _isFightFinished = true;
            PlayFightTimeline(bossAttackPlaybackDirector);
        }
        else
        {
            ShowBattleMenu();
        }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is ShowStatusMessageMarker showMessage)
        {
            uiStatusMessage.ShowMessage(showMessage.text);
            PlaySfx(showMessage.sfx);
        }
        else if (notification is ApplyDamageMarker applyDamage)
        {
            var target = applyDamage.target == JrpgTarget.Boss ? bossTargetPoint : playerTargetPoint;
            var damageText = applyDamage.customText.Length == 0 ? Mathf.Abs(applyDamage.damageAmount).ToString() : applyDamage.customText;

            var color = applyDamage.damageAmount >= 0 ? Color.white : new Color(0f, 1f, 0.5f);
            uiDamageNumbers.Show(target, damageText, color);

            guyCurrentHp -= applyDamage.damageAmount;

            if (guyCurrentHp < 0)
                guyCurrentHp = 0;
            
            uiHealthPanel.SetHp(guyMaxHp, guyCurrentHp, true);

            if (applyDamage.target == JrpgTarget.Player && applyDamage.damageAmount > 0)
                ApplyDamageToPlayer();
            
            PlaySfx(applyDamage.sfx);
        } 
        else if (notification is ChangeHealthVisibilityMarker showDamage)
        {
            if (showDamage.show)
            {
                uiHealthPanel.Show();
            }
            else
            {
                uiHealthPanel.Hide();
            }
        } 
        else if (notification is FadeOutMusicMarker fadeOutMusicMarker)
        {
            ServiceLocator.Get<AudioService>()
                .StopBackgroundMusic(fadeOutMusicMarker.duration);
        }
        else if (notification is FightFinishedMarker)
        {
            ServiceLocator.Get<SceneLoadingService>()
                .LoadScene(nextScene, nextCurtain);
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (!clip) 
            return;
        
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void HideBattleMenu()
    {
        uiAction1Layer.Hide();
        uiAction2Layer.Hide();
    }
    
    public void ShowBattleMenu()
    {
        PlaySfx(clipMenuOpen);

        uiAction1Layer.SetInteractable(true);
        uiAction1Layer.Show("Actions", new UiJrpgButtonPanel.Entry
            {
                name = "Attack"
            },
            new UiJrpgButtonPanel.Entry
            {
                name = "Defend"
            },
            new UiJrpgButtonPanel.Entry
            {
                name = "Excuse",
                onClick = GetAttackClickAction()
            },
            new UiJrpgButtonPanel.Entry
            {
                name = "Items",
                onClick = GetItemClickAction()
            }
        );
    }

    private Action GetAttackClickAction()
    {
        var items = new List<UiJrpgButtonPanel.Entry>();

        if (!_isAttack1Played)
        {
            items.Add(new UiJrpgButtonPanel.Entry
            {
                name = "I'm irreplaceable!",
                onClick = () =>
                {
                    _isAttack1Played = true;
                    PlayFightTimeline(attack1Director);
                }
            });
        }

        if (!_isAttack2Played)
        {
            items.Add(new UiJrpgButtonPanel.Entry
            {
                name = "I'm not that late!",
                onClick = () =>
                {
                    _isAttack2Played = true;
                    PlayFightTimeline(attack2Director);
                }
            });
        }
        
        if (!_isAttack3Played)
        {
            items.Add(new UiJrpgButtonPanel.Entry
            {
                name = "You are always late too!",
                onClick = () =>
                {
                    _isAttack3Played = true;
                    PlayFightTimeline(attack3Director);
                }
            });
        }
        
        if (!_isAttack4Played)
        {
            items.Add(new UiJrpgButtonPanel.Entry
            {
                name = "Can we just forget about it?",
                onClick = () =>
                {
                    _isAttack4Played = true;
                    PlayFightTimeline(attack4Director);
                }
            });
        }

        return CreateShowItemsInSecondaryMenuAction(2, "Arguments", items);
    }

    private Action GetItemClickAction()
    {
        var items = new List<UiJrpgButtonPanel.Entry>();
        
        if (_hasSandwich)
        {
            items.Add(new UiJrpgButtonPanel.Entry
            {
                name = "Sandwich",
                onClick = () =>
                {
                    _hasSandwich = false;
                    PlayFightTimeline(eatSandwichDirector);
                }
            });
        }

        return CreateShowItemsInSecondaryMenuAction(3, "Items", items);
    }

    private Action CreateShowItemsInSecondaryMenuAction(int returnButtonIndex, string title, List<UiJrpgButtonPanel.Entry> items)
    {
        return items.Count == 0 ? null : () =>
        {
            PlaySfx(clipMenuOpen);
            uiAction1Layer.SetInteractable(false);
            uiAction2Layer.SetInteractable(true);
            uiAction2Layer.onCancelListener = () =>
            {
                PlaySfx(clipMenuClose);
                uiAction1Layer.SetInteractable(true);
                uiAction1Layer.SelectButton(returnButtonIndex);
            };
                    
            uiAction2Layer.Show(title, items.ToArray());
        };
    }
}
