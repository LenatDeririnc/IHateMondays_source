using DG.Tweening;
using Plugins.ServiceLocator;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Services
{
    public class AudioService : Service
    {
        [FormerlySerializedAs("_loopSource")] [SerializeField] 
        private AudioSource _musicLoopSource;
        
        [SerializeField] 
        [FormerlySerializedAs("_intoSource")] 
        [FormerlySerializedAs("_introSource")] 
        private AudioSource _musicIntroSource;
        
        [SerializeField]
        private AudioSource _ambientSource;

        [SerializeField] 
        private AudioMixer _audioMixer;
        
        private Sequence _fadeSequence;
        private float _fadeTargetAmount = -1;

        public float MasterVolume
        {
            get { _audioMixer.GetFloat("MasterVolume", out var v); return v; }
            set => _audioMixer.SetFloat("MasterVolume", value);
        }
        
        public float MusicVolume
        {
            get { _audioMixer.GetFloat("MusicVolume", out var v); return v; }
            set => _audioMixer.SetFloat("MusicVolume", value);
        }
        
        public float SfxVolume
        {
            get { _audioMixer.GetFloat("SfxVolume", out var v); return v; }
            set => _audioMixer.SetFloat("SfxVolume", value);
        }
        
        public float VoiceVolume
        {
            get { _audioMixer.GetFloat("VoiceVolume", out var v); return v; }
            set => _audioMixer.SetFloat("VoiceVolume", value);
        }

        public void FadeMusicVolume(float targetVolume, float duration)
        {
            FadeMusicVolume(targetVolume, duration, null);
        }

        public void PlayBackgroundMusic(AudioClip intro, AudioClip loop, AudioClip ambient = null,
            float fadeOutDuration = 0, float fadeInDuration = 0f, 
            bool startFromBeginning = false, float stopMusicFade = 1f)
        {
            if (intro == null && loop == null && ambient == null) {
                if (IsBackgroundMusicPlaying()) 
                {
                    if (!startFromBeginning && 
                        _musicLoopSource.clip == loop && 
                        _musicIntroSource.clip == intro && 
                        _fadeTargetAmount > 0f)
                        return;
                
                    FadeMusicVolume(0f, fadeOutDuration, StartPlayback);
                }
                else
                {
                    StopBackgroundMusic(stopMusicFade);
                }
            }
            
            void StartPlayback()
            {
                _musicLoopSource.Stop();
                _musicIntroSource.Stop();
                _ambientSource.Stop();
                _musicLoopSource.clip = loop;

                if (intro)
                {
                    _musicIntroSource.clip = intro;
                    _musicIntroSource.Play();
                    _musicLoopSource.PlayDelayed(intro.length);
                }
                else
                {
                    _musicLoopSource.Play();
                }

                if (ambient) 
                {
                    _ambientSource.clip = ambient;
                    _ambientSource.Play();
                }

                FadeMusicVolume(1f, fadeInDuration);
            }

            if (IsBackgroundMusicPlaying()) 
            {
                if (!startFromBeginning && 
                    _musicLoopSource.clip == loop && 
                    _musicIntroSource.clip == intro && 
                    _fadeTargetAmount > 0f)
                    return;
                
                FadeMusicVolume(0f, fadeOutDuration, StartPlayback);
            }
            else 
            {
                StartPlayback();
            }
        }

        public void StopBackgroundMusic(float fadeOutDuration)
        {
            FadeMusicVolume(0f, fadeOutDuration, () =>
            {
                _musicIntroSource.Stop();
                _musicLoopSource.Stop();
            });
        }
        
        private bool IsBackgroundMusicPlaying()
        {
            var hasAnyVolume = _musicIntroSource.volume > 0f && _musicLoopSource.volume > 0f && _ambientSource.volume > 0f;
            var isSomethingPlaying = _musicIntroSource.isPlaying || _musicLoopSource.isPlaying || _ambientSource.isPlaying;
            return hasAnyVolume && isSomethingPlaying;
        }
        
        private void FadeMusicVolume(float volume, float duration, TweenCallback onComplete = null)
        {
            // Запускаем анимацию только в случае если target volume последнего fade не совпадает
            if (!Mathf.Approximately(_fadeTargetAmount, volume))
            {
                DOTween.Kill(this);
                _fadeSequence = DOTween.Sequence()
                    .Join(_musicIntroSource.DOFade(volume, duration))
                    .Join(_musicLoopSource.DOFade(volume, duration))
                    .Join(_ambientSource.DOFade(volume, duration))
                    .SetTarget(this);
                
                Debug.Log($"Fade music to: {volume}");
            }

            _fadeTargetAmount = volume;

            if (_fadeSequence != null)
            {
                _fadeSequence.OnComplete(() =>
                {
                    _fadeSequence = null;
                    onComplete?.Invoke();
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }
    }
}