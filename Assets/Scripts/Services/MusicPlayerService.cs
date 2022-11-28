using DG.Tweening;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class MusicPlayerService : Service, IAwakeService
    {
        [SerializeField] private AudioSource _loopSource;
        [SerializeField] private AudioSource _intoSource;
        [SerializeField] [Range(0, 1)] private float _defaultVolume = 1;

        private AudioSource _currentPlaying =>
            _loopSource.isPlaying ? _loopSource : 
            _intoSource.isPlaying ? _intoSource : null;

        public void AwakeService()
        {
            _intoSource.volume = _defaultVolume;
            _loopSource.volume = _defaultVolume;
        }

        public void SetVolume(float volume)
        {
            _loopSource.volume = volume;
            _intoSource.volume = volume;
        }

        public void PlayLoop(AudioClip loop, float durationEnter = 0, float durationExit = 0, bool isReplay = false)
        {
            if (!isReplay && _loopSource.clip == loop)
                return;

            void OnSeqOnComplete()
            {
                _loopSource.Stop();
                _intoSource.Stop();

                _loopSource.clip = loop;
                _loopSource.Play();
                Volume1(durationExit);
            }
            
            if (_loopSource.isPlaying) {
                var seq = Volume0(durationEnter);
                seq.onComplete += OnSeqOnComplete;
            }
            else {
                OnSeqOnComplete();
            }
        }

        public void PlayLoopWithIntro(AudioClip intro, AudioClip loop, float durationEnter = 0, float durationExit = 0, bool isReplay = false)
        {
            if (!isReplay && (_loopSource.clip == loop && _intoSource.clip == intro))
                return;

            void OnSeqOnComplete()
            {
                _loopSource.Stop();
                _intoSource.Stop();

                _intoSource.clip = intro;
                _loopSource.clip = loop;
                _intoSource.Play();
                _loopSource.PlayDelayed(intro.length);
                Volume1(durationExit);
            }

            if (_currentPlaying != null && _currentPlaying.isPlaying) {
                var seq = Volume0(durationEnter);
                seq.onComplete += OnSeqOnComplete;
            }
            else {
                OnSeqOnComplete();
            }
        }

        public void StopPlay(float duration)
        {
            var seq = Volume0(duration);
            seq.onComplete += () =>
            {
                _intoSource.Stop();
                _loopSource.Stop();
            };
        }

        public Sequence Volume1(float duration = 0)
        {
            var seq = DOTween.Sequence();
            seq.Join(DOTween.To(() => _intoSource.volume, _ => _intoSource.volume = _, _defaultVolume, duration));
            seq.Join(DOTween.To(() => _loopSource.volume, _ => _loopSource.volume = _, _defaultVolume, duration));
            return seq;
        }

        public Sequence Volume0(float duration = 0)
        {
            var seq = DOTween.Sequence();
            seq.Join(DOTween.To(() => _intoSource.volume, _ => _intoSource.volume = _, 0, duration));
            seq.Join(DOTween.To(() => _loopSource.volume, _ => _loopSource.volume = _, 0, duration));
            return seq;
        }
    }
}